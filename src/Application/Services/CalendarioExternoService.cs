using PousadaApi.Application.DTOs;
using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Constants;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class CalendarioExternoService : ICalendarioExternoService
{
    private readonly ICalendarioExternoRepository _calendarioRepository;
    private readonly IQuartoRepository _quartoRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IHospedeRepository _hospedeRepository;
    private readonly IIcalFeedClient _feedClient;
    private readonly IIcalParser _parser;
    private readonly ICurrentUserService _currentUser;

    public CalendarioExternoService(
        ICalendarioExternoRepository calendarioRepository,
        IQuartoRepository quartoRepository,
        IReservaRepository reservaRepository,
        IHospedeRepository hospedeRepository,
        IIcalFeedClient feedClient,
        IIcalParser parser,
        ICurrentUserService currentUser)
    {
        _calendarioRepository = calendarioRepository;
        _quartoRepository = quartoRepository;
        _reservaRepository = reservaRepository;
        _hospedeRepository = hospedeRepository;
        _feedClient = feedClient;
        _parser = parser;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<CalendarioExternoReadDto>> ListarAsync(int quartoId, CancellationToken cancellationToken = default)
    {
        await ValidarQuartoAsync(quartoId, cancellationToken);
        var itens = await _calendarioRepository.ListarPorQuartoEUsuarioAsync(quartoId, _currentUser.UserId, cancellationToken);
        return itens.Select(MapRead);
    }

    public async Task<CalendarioExternoReadDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cal = await _calendarioRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        return cal is null ? null : MapRead(cal);
    }

    public async Task<CalendarioExternoReadDto> CriarAsync(CalendarioExternoCreateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidarQuartoAsync(dto.QuartoId, cancellationToken);

        var calendario = new CalendarioExterno
        {
            QuartoId = dto.QuartoId,
            Canal = NormalizarCanal(dto.Canal),
            UrlImportacao = dto.UrlImportacao.Trim(),
            Ativo = true
        };

        await _calendarioRepository.AdicionarAsync(calendario, cancellationToken);
        return MapRead(calendario);
    }

    public async Task AtualizarAsync(int id, CalendarioExternoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (id != dto.Id)
            throw new InvalidOperationException("ID não corresponde.");

        var calendario = await _calendarioRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (calendario is null)
            throw new AcessoNegadoException();

        calendario.Canal = NormalizarCanal(dto.Canal);
        calendario.UrlImportacao = dto.UrlImportacao.Trim();
        calendario.Ativo = dto.Ativo;

        await _calendarioRepository.AtualizarAsync(calendario, cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var calendario = await _calendarioRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (calendario is null)
            throw new AcessoNegadoException();

        await _calendarioRepository.RemoverAsync(calendario, cancellationToken);
    }

    public async Task<CalendarioSyncResultDto> SincronizarAsync(int id, CancellationToken cancellationToken = default)
    {
        var calendario = await _calendarioRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (calendario is null)
            throw new AcessoNegadoException();

        return await ExecuteSyncAsync(calendario, cancellationToken);
    }

    public async Task SincronizarTodosAtivosAsync(CancellationToken cancellationToken = default)
    {
        var calendarios = await _calendarioRepository.ListarAtivosAsync(cancellationToken);
        foreach (var calendario in calendarios)
        {
            try
            {
                await ExecuteSyncAsync(calendario, cancellationToken);
            }
            catch
            {
                // Erro registrado em UltimoErro pelo ExecuteSyncAsync
            }
        }
    }

    private async Task<CalendarioSyncResultDto> ExecuteSyncAsync(CalendarioExterno calendario, CancellationToken cancellationToken)
    {
        var resultado = new CalendarioSyncResultDto();

        try
        {
            var conteudo = await _feedClient.BaixarAsync(calendario.UrlImportacao, cancellationToken);
            var eventos = _parser.Parse(conteudo);
            var quarto = calendario.Quarto ?? await _quartoRepository.ObterPorIdAsync(calendario.QuartoId, cancellationToken);
            if (quarto is null)
                throw new InvalidOperationException("Quarto não encontrado.");

            var uidsNoFeed = new HashSet<string>(StringComparer.Ordinal);

            foreach (var evt in eventos)
            {
                if (evt.Cancelado)
                    continue;

                uidsNoFeed.Add(evt.Uid);
                var ehBloqueioIndisponibilidade = EhBloqueioIndisponibilidade(calendario, evt);
                var hospedeId = await ObterOuCriarHospedeCanalAsync(
                    quarto.PousadaId,
                    calendario.Canal,
                    ehBloqueioIndisponibilidade,
                    cancellationToken);
                var status = ehBloqueioIndisponibilidade ? ReservaStatus.Bloqueada : ReservaStatus.Confirmada;
                var titulo = ehBloqueioIndisponibilidade ? $"Bloqueio {calendario.Canal}" : evt.Titulo;
                var observacoes = ehBloqueioIndisponibilidade
                    ? $"Periodo indisponivel importado de {calendario.Canal}"
                    : $"Importado de {calendario.Canal}";

                var existente = await _reservaRepository.ObterPorUidExternoAsync(calendario.QuartoId, evt.Uid, cancellationToken);
                if (existente is not null && existente.Origem == ReservaOrigens.Manual)
                {
                    resultado.Ignorados++;
                    continue;
                }

                if (existente is null)
                {
                    var overlap = await _reservaRepository.ExisteSobreposicaoNoQuartoAsync(
                        calendario.QuartoId, evt.DataInicio, evt.DataFim, null, cancellationToken);
                    if (overlap)
                    {
                        resultado.Ignorados++;
                        continue;
                    }

                    var reserva = new Reserva
                    {
                        QuartoId = calendario.QuartoId,
                        HospedeId = hospedeId,
                        DataEntrada = evt.DataInicio,
                        DataSaida = evt.DataFim,
                        Status = status,
                        ValorTotal = 0,
                        Origem = calendario.Canal,
                        UidExterno = evt.Uid,
                        CalendarioExternoId = calendario.Id,
                        TituloExterno = titulo,
                        SincronizadoEm = DateTime.UtcNow,
                        Observacoes = observacoes
                    };
                    await _reservaRepository.AdicionarAsync(reserva, cancellationToken);
                    resultado.Criados++;
                }
                else
                {
                    existente.HospedeId = hospedeId;
                    existente.DataEntrada = evt.DataInicio;
                    existente.DataSaida = evt.DataFim;
                    existente.TituloExterno = titulo;
                    existente.SincronizadoEm = DateTime.UtcNow;
                    existente.Status = status;
                    existente.Observacoes = observacoes;
                    await _reservaRepository.AtualizarAsync(existente, cancellationToken);
                    resultado.Atualizados++;
                }
            }

            var importadas = await _reservaRepository.ListarPorCalendarioExternoAsync(calendario.Id, cancellationToken);
            foreach (var reserva in importadas)
            {
                if (string.IsNullOrEmpty(reserva.UidExterno) || uidsNoFeed.Contains(reserva.UidExterno))
                    continue;

                var rastreada = await _reservaRepository.ObterPorIdRastreadoAsync(reserva.Id, cancellationToken);
                if (rastreada is null)
                    continue;

                rastreada.Status = ReservaStatus.Cancelada;
                rastreada.SincronizadoEm = DateTime.UtcNow;
                await _reservaRepository.AtualizarAsync(rastreada, cancellationToken);
                resultado.Cancelados++;
            }

            calendario.UltimaSincronizacao = DateTime.UtcNow;
            calendario.UltimoErro = null;
        }
        catch (Exception ex)
        {
            calendario.UltimoErro = ex.Message;
            throw;
        }
        finally
        {
            await _calendarioRepository.AtualizarAsync(calendario, cancellationToken);
        }

        return resultado;
    }

    private async Task ValidarQuartoAsync(int quartoId, CancellationToken cancellationToken)
    {
        var quarto = await _quartoRepository.ObterPorIdEUsuarioAsync(quartoId, _currentUser.UserId, cancellationToken);
        if (quarto is null)
            throw new AcessoNegadoException();
    }

    private async Task<int> ObterOuCriarHospedeCanalAsync(
        int pousadaId,
        string canal,
        bool bloqueioIndisponibilidade,
        CancellationToken cancellationToken)
    {
        var nome = bloqueioIndisponibilidade
            ? $"Bloqueio {canal}"
            : canal switch
        {
            ReservaOrigens.Airbnb => "Reserva Airbnb",
            ReservaOrigens.Booking => "Reserva Booking",
            _ => $"Reserva {canal}"
        };

        var existente = await _hospedeRepository.ObterPorNomeEPousadaAsync(nome, pousadaId, cancellationToken);
        if (existente is not null)
            return existente.Id;

        var hospede = new Hospede { PousadaId = pousadaId, Nome = nome };
        await _hospedeRepository.AdicionarAsync(hospede, cancellationToken);
        return hospede.Id;
    }

    private static bool EhBloqueioIndisponibilidade(CalendarioExterno calendario, IcalEventoDto evt)
    {
        if (string.IsNullOrWhiteSpace(evt.Titulo))
            return false;

        return calendario.Canal switch
        {
            ReservaOrigens.Airbnb => evt.Titulo.Contains("Not available", StringComparison.OrdinalIgnoreCase),
            ReservaOrigens.Booking => evt.Titulo.Contains("Not available", StringComparison.OrdinalIgnoreCase)
                || evt.Titulo.StartsWith("CLOSED", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static string NormalizarCanal(string canal) =>
        canal.Trim() switch
        {
            var c when c.Equals(ReservaOrigens.Airbnb, StringComparison.OrdinalIgnoreCase) => ReservaOrigens.Airbnb,
            var c when c.Equals(ReservaOrigens.Booking, StringComparison.OrdinalIgnoreCase) => ReservaOrigens.Booking,
            _ => ReservaOrigens.Outro
        };

    private static CalendarioExternoReadDto MapRead(CalendarioExterno c) => new()
    {
        Id = c.Id,
        QuartoId = c.QuartoId,
        Canal = c.Canal,
        UrlImportacao = c.UrlImportacao,
        Ativo = c.Ativo,
        UltimaSincronizacao = c.UltimaSincronizacao,
        UltimoErro = c.UltimoErro
    };
}
