using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public interface IHospedeService
{
    Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
