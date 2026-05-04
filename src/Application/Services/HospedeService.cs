using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public class HospedeService : IHospedeService
{
    public Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
