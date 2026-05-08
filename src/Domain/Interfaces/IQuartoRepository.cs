using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IQuartoRepository
{
    Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorIdComPousadaAsync(int id, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> PousadaExisteAsync(int pousadaId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
