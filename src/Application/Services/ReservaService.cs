using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IQuartoRepository _quartoRepository;

    public ReservaService(IReservaRepository reservaRepository, IQuartoRepository quartoRepository)
    {
        _reservaRepository = reservaRepository;
        _quartoRepository = quartoRepository;
    }

    public Task<IEnumerable<Reserva>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        return _reservaRepository.ListarComRelacionamentosAsync(pousadaId, cancellationToken);
    }

    public Task<Reserva?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _reservaRepository.ObterPorIdComRelacionamentosAsync(id, cancellationToken);
    }

    public async Task<Reserva> CriarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        var quarto = await _quartoRepository.ObterPorIdAsync(reserva.QuartoId, cancellationToken);
        if (quarto == null) throw new InvalidOperationException("Quarto não encontrado.");

        var disponivel = await QuartoDisponivelAsync(reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, null, cancellationToken);
        if (!disponivel) throw new InvalidOperationException("Quarto indisponível no período informado.");

        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;
        reserva.Status = reserva.Status ?? "Confirmada";

        await _reservaRepository.AdicionarAsync(reserva, cancellationToken);
        return reserva;
    }

    public async Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        var quarto = await _quartoRepository.ObterPorIdAsync(reserva.QuartoId, cancellationToken);
        if (quarto == null) throw new InvalidOperationException("Quarto não encontrado.");

        var disponivel = await QuartoDisponivelAsync(reserva.QuartoId, reserva.DataEntrada, reserva.DataSaida, reserva.Id, cancellationToken);
        if (!disponivel) throw new InvalidOperationException("Quarto indisponível no período informado.");

        var nights = (int)(reserva.DataSaida.Date - reserva.DataEntrada.Date).TotalDays;
        if (nights < 1) nights = 1;
        reserva.ValorTotal = nights * quarto.ValorDiaria;

        await _reservaRepository.AtualizarAsync(reserva, cancellationToken);
    }

    public async Task CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        var reserva = await _reservaRepository.ObterPorIdRastreadoAsync(id, cancellationToken);
        if (reserva == null) return;

        reserva.Status = "Cancelada";
        await _reservaRepository.AtualizarAsync(reserva, cancellationToken);
    }

    public async Task<bool> QuartoDisponivelAsync(int quartoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdIgnorar = null, CancellationToken cancellationToken = default)
    {
        if (dataEntrada >= dataSaida) return false;

        var overlap = await _reservaRepository.ExisteSobreposicaoNoQuartoAsync(
            quartoId, dataEntrada, dataSaida, reservaIdIgnorar, cancellationToken);
        return !overlap;
    }
}
