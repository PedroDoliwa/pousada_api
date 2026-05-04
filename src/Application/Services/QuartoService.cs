using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public class QuartoService : IQuartoService
{
    public Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Quarto> CriarAsync(Quarto quarto, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
