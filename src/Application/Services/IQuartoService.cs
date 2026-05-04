using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Services;

public interface IQuartoService
{
    Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default);
    Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Quarto> CriarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
