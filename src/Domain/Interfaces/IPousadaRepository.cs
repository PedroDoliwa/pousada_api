using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IPousadaRepository
{
    Task<IEnumerable<Pousada>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<Pousada?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default);
    Task<bool> PertenceAoUsuarioAsync(int pousadaId, int usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
