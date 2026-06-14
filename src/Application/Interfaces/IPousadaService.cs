using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IPousadaService
{
    Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task SalvarFotoAsync(int id, byte[] bytes, string contentType, CancellationToken cancellationToken = default);
    Task<(byte[]? Bytes, string? ContentType)> ObterFotoAsync(int id, CancellationToken cancellationToken = default);
    Task RemoverFotoAsync(int id, CancellationToken cancellationToken = default);
}
