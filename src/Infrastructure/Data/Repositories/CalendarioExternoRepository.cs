using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class CalendarioExternoRepository : ICalendarioExternoRepository
{
    private readonly PousadaDbContext _db;

    public CalendarioExternoRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CalendarioExterno>> ListarPorQuartoEUsuarioAsync(int quartoId, int usuarioId, CancellationToken cancellationToken = default)
    {
        return await _db.CalendariosExternos
            .Include(c => c.Quarto)
            .ThenInclude(q => q!.Pousada)
            .AsNoTracking()
            .Where(c => c.QuartoId == quartoId && c.Quarto != null && c.Quarto.Pousada != null && c.Quarto.Pousada.UsuarioId == usuarioId)
            .OrderBy(c => c.Canal)
            .ToListAsync(cancellationToken);
    }

    public async Task<CalendarioExterno?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default)
    {
        return await _db.CalendariosExternos
            .Include(c => c.Quarto)
            .ThenInclude(q => q!.Pousada)
            .FirstOrDefaultAsync(
                c => c.Id == id && c.Quarto != null && c.Quarto.Pousada != null && c.Quarto.Pousada.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task AdicionarAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default)
    {
        _db.CalendariosExternos.Add(calendario);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default)
    {
        _db.CalendariosExternos.Update(calendario);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(CalendarioExterno calendario, CancellationToken cancellationToken = default)
    {
        _db.CalendariosExternos.Remove(calendario);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<CalendarioExterno>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _db.CalendariosExternos
            .Include(c => c.Quarto)
            .ThenInclude(q => q!.Pousada)
            .Where(c => c.Ativo)
            .ToListAsync(cancellationToken);
    }
}
