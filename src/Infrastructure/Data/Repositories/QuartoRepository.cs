using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class QuartoRepository : IQuartoRepository
{
    private readonly PousadaDbContext _db;

    public QuartoRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId, CancellationToken cancellationToken = default)
    {
        var query = _db.Quartos.Include(q => q.Pousada).AsNoTracking().AsQueryable();

        if (pousadaId.HasValue)
            query = query.Where(q => q.PousadaId == pousadaId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<Quarto?> ObterPorIdComPousadaAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Quartos
            .Include(q => q.Pousada)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Quartos.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public Task<bool> PousadaExisteAsync(int pousadaId, CancellationToken cancellationToken = default)
    {
        return _db.Pousadas.AnyAsync(p => p.Id == pousadaId, cancellationToken);
    }

    public async Task AdicionarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        _db.Quartos.Add(quarto);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        _db.Quartos.Update(quarto);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var quarto = await _db.Quartos.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (quarto is null)
            return;

        _db.Quartos.Remove(quarto);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
