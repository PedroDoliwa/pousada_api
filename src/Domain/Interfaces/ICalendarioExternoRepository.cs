using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface ICalendarioExternoRepository
{
    Task<IEnumerable<CalendarioExterno>> ListarPorQuartoEUsuarioAsync(int quartoId, int usuarioId, CancellationToken cancellationToken = default);
    Task<CalendarioExterno?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default);
    Task AtualizarAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default);
    Task RemoverAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default);
    Task<IEnumerable<CalendarioExterno>> ListarAtivosAsync(CancellationToken cancellationToken = default);
}
