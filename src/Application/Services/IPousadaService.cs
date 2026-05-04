using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public interface IPousadaService
{
    Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
