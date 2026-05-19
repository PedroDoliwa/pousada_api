using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IHospedeService
{
    Task<IEnumerable<Hospede>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default);
    Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
