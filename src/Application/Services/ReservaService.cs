using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Constants;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IQuartoRepository _quartoRepository;
    private readonly IHospedeRepository _hospedeRepository;
    private readonly IPousadaRepository _pousadaRepository;
    private readonly IDisponibilidadeService _disponibilidade;
    private readonly ICurrentUserService _currentUser;

    public ReservaService(
        IReservaRepository reservaRepository,
        IQuartoRepository quartoRepository,
        IHospedeRepository hospedeRepository,
        IPousadaRepository pousadaRepository,
        IDisponibilidadeService disponibilidade,
        ICurrentUserService currentUser)
    {
        _reservaRepository = reservaRepository;
        _quartoRepository = quartoRepository;
        _hospedeRepository = hospedeRepository;
        _pousadaRepository = pousadaRepository;
        _disponibilidade = disponibilidade;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<Reserva>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        if (pousadaId.HasValue)
        {
            var pertence = await _pousadaRepository.PertenceAoUsuarioAsync(pousadaId.Value, _currentUser.UserId, cancellationToken);
            if (!pertence)
                throw new AcessoNegadoException();
        }

        return await _reservaRepository.ListarPorUsuarioAsync(_currentUser.UserId, pousadaId, cancellationToken);
    }

    public Task<Reserva?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _reservaRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
    }

    public async Task<Reserva> CriarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        reserva.DataEntrada = ToUtc(reserva.DataEntrada);
        reserva.DataSaida = ToUtc(reserva.DataSaida);

        var quarto = await _quartoRepository.ObterPorIdEUsuarioAsync(reserva.QuartoId, _currentUser.UserId, cancellationToken);
        if (quarto == null)
            throw new AcessoNegadoException();

        var hospede = await _hospedeRepository.ObterPorIdEUsuarioAsync(reserva.HospedeId, _currentUser.UserId, cancellationToken);
        if (hospede == null)
            throw new AcessoNegadoException();

        if (hospede.PousadaId != quarto.PousadaId)
            throw new InvalidOperationException("Hóspede não pertence à mesma pousada do quarto.");

        var disponivel = await _disponibilidade.QuartoDisponivelAsync(
            reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, null, cancellationToken);
        if (!disponivel)
            throw new InvalidOperationException("Quarto indisponível no período informado.");

        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;
        reserva.Status = reserva.Status ?? "Confirmada";
        reserva.Origem = ReservaOrigens.Manual;

        await _reservaRepository.AdicionarAsync(reserva, cancellationToken);
        return reserva;
    }

    public async Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        reserva.DataEntrada = ToUtc(reserva.DataEntrada);
        reserva.DataSaida = ToUtc(reserva.DataSaida);

        var existente = await _reservaRepository.ObterPorIdEUsuarioAsync(reserva.Id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        var quarto = await _quartoRepository.ObterPorIdEUsuarioAsync(reserva.QuartoId, _currentUser.UserId, cancellationToken);
        if (quarto == null)
            throw new AcessoNegadoException();

        var hospede = await _hospedeRepository.ObterPorIdEUsuarioAsync(reserva.HospedeId, _currentUser.UserId, cancellationToken);
        if (hospede == null)
            throw new AcessoNegadoException();

        if (hospede.PousadaId != quarto.PousadaId)
            throw new InvalidOperationException("Hóspede não pertence à mesma pousada do quarto.");

        var disponivel = await _disponibilidade.QuartoDisponivelAsync(
            reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, reserva.Id, cancellationToken);
        if (!disponivel)
            throw new InvalidOperationException("Quarto indisponível no período informado.");

        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;

        await _reservaRepository.AtualizarAsync(reserva, cancellationToken);
    }

    public async Task CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        var reserva = await _reservaRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (reserva is null)
            throw new AcessoNegadoException();

        var rastreada = await _reservaRepository.ObterPorIdRastreadoAsync(id, cancellationToken);
        if (rastreada is null)
            throw new AcessoNegadoException();

        rastreada.Status = "Cancelada";
        await _reservaRepository.AtualizarAsync(rastreada, cancellationToken);
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}
