using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public class ReservaService : IReservaService
{
    public Task<IEnumerable<Reserva>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Reserva?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Reserva> CriarAsync(Reserva reserva, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task CancelarAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<bool> QuartoDisponivelAsync(int quartoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdIgnorar = null, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
