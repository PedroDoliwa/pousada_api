using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class PousadaRepository : IPousadaRepository
{
    private readonly PousadaDbContext _db;

    public PousadaRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Pousada>> ListarComQuartosAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Pousadas
            .Include(p => p.Quartos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<Pousada?> ObterPorIdComQuartosAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Pousadas
            .Include(p => p.Quartos)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        _db.Pousadas.Add(pousada);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        _db.Pousadas.Update(pousada);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pousada = await _db.Pousadas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pousada is null)
            return;

        _db.Pousadas.Remove(pousada);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
