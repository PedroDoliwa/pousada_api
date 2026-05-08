using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IPousadaRepository
{
    Task<IEnumerable<Pousada>> ListarComQuartosAsync(CancellationToken cancellationToken = default);
    Task<Pousada?> ObterPorIdComQuartosAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
