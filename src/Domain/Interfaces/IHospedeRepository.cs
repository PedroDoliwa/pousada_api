using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IHospedeRepository
{
    Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
