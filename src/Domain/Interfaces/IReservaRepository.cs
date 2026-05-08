using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IReservaRepository
{
    Task<IEnumerable<Reserva>> ListarComRelacionamentosAsync(int? pousadaId, CancellationToken cancellationToken = default);
    Task<Reserva?> ObterPorIdComRelacionamentosAsync(int id, CancellationToken cancellationToken = default);
    Task<Reserva?> ObterPorIdRastreadoAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default);
    /// <summary>Retorna true se existir reserva não cancelada com sobreposição de datas no quarto.</summary>
    Task<bool> ExisteSobreposicaoNoQuartoAsync(
        int quartoId,
        DateTime dataEntrada,
        DateTime dataSaida,
        int? ignorarReservaId,
        CancellationToken cancellationToken = default);
}
