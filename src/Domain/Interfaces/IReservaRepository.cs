using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> ListarPorUsuarioAsync(int usuarioId, int? pousadaId, CancellationToken cancellationToken = default);
    Task<Reserva?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default);
    Task<Reserva?> ObterPorIdRastreadoAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task<bool> ExisteSobreposicaoNoQuartoAsync(
        int quartoId,
        DateTime dataEntrada,
        DateTime dataSaida,
        int? ignorarReservaId,
        CancellationToken cancellationToken = default);
}
