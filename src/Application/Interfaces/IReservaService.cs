using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IReservaService
{
    Task<IEnumerable<Reserva>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default);
    Task<Reserva?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Reserva> CriarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task CancelarAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> QuartoDisponivelAsync(int quartoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdIgnorar = null, CancellationToken cancellationToken = default);
}
