using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IHospedeRepository
{
    Task<IEnumerable<Hospede>> ListarPorUsuarioAsync(int usuarioId, int? pousadaId, CancellationToken cancellationToken = default);
    Task<Hospede?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default);
    Task<bool> PousadaPertenceAoUsuarioAsync(int pousadaId, int usuarioId, CancellationToken cancellationToken = default);
    Task<Hospede?> ObterPorNomeEPousadaAsync(string nome, int pousadaId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
