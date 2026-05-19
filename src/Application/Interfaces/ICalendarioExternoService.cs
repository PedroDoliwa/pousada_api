using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Interfaces;

public interface ICalendarioExternoService
{
    Task<IEnumerable<CalendarioExternoReadDto>> ListarAsync(int quartoId, CancellationToken cancellationToken = default);
    Task<CalendarioExternoReadDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CalendarioExternoReadDto> CriarAsync(CalendarioExternoCreateDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, CalendarioExternoUpdateDto dto, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<CalendarioSyncResultDto> SincronizarAsync(int id, CancellationToken cancellationToken = default);
    Task SincronizarTodosAtivosAsync(CancellationToken cancellationToken = default);
}
