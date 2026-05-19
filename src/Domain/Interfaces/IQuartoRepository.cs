using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IQuartoRepository
{
    Task<IEnumerable<Quarto>> ListarPorUsuarioAsync(int usuarioId, int? pousadaId, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorTokenExportacaoAsync(string tokenExportacao, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default);
}
