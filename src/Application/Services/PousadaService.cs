using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public class PousadaService : IPousadaService
{
    public Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
