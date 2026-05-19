using PousadaApi.Application.DTOs;
using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class DisponibilidadeService : IDisponibilidadeService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IQuartoRepository _quartoRepository;
    private readonly IPousadaRepository _pousadaRepository;
    private readonly ICurrentUserService _currentUser;

    public DisponibilidadeService(
        IReservaRepository reservaRepository,
        IQuartoRepository quartoRepository,
        IPousadaRepository pousadaRepository,
        ICurrentUserService currentUser)
    {
        _reservaRepository = reservaRepository;
        _quartoRepository = quartoRepository;
        _pousadaRepository = pousadaRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> QuartoDisponivelAsync(
        int quartoId,
        DateTime dataEntrada,
        DateTime dataSaida,
        int? reservaIdIgnorar = null,
        CancellationToken cancellationToken = default)
    {
        dataEntrada = ToUtc(dataEntrada);
        dataSaida = ToUtc(dataSaida);

        if (dataEntrada >= dataSaida)
            return false;

        var quarto = await _quartoRepository.ObterPorIdEUsuarioAsync(quartoId, _currentUser.UserId, cancellationToken);
        if (quarto is null)
            return false;

        var overlap = await _reservaRepository.ExisteSobreposicaoNoQuartoAsync(
            quartoId, dataEntrada, dataSaida, reservaIdIgnorar, cancellationToken);
        return !overlap;
    }

    public async Task<IEnumerable<OcupacaoReadDto>> ListarOcupacaoAsync(
        int pousadaId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default)
    {
        de = ToUtc(de);
        ate = ToUtc(ate);

        if (de >= ate)
            throw new InvalidOperationException("A data final deve ser posterior à data inicial.");

        var pertence = await _pousadaRepository.PertenceAoUsuarioAsync(pousadaId, _currentUser.UserId, cancellationToken);
        if (!pertence)
            throw new AcessoNegadoException();

        var reservas = await _reservaRepository.ListarOcupacaoPorPousadaAsync(
            _currentUser.UserId, pousadaId, de, ate, cancellationToken);

        return reservas.Select(r => new OcupacaoReadDto
        {
            ReservaId = r.Id,
            QuartoId = r.QuartoId,
            QuartoNumeroOuNome = r.Quarto?.NumeroOuNome ?? string.Empty,
            HospedeId = r.HospedeId,
            HospedeNome = r.Hospede?.Nome ?? string.Empty,
            DataEntrada = r.DataEntrada,
            DataSaida = r.DataSaida,
            Status = r.Status,
            Origem = r.Origem
        });
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}
