using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class HospedeRepository : IHospedeRepository
{
    private readonly PousadaDbContext _db;

    public HospedeRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Hospedes
            .AsNoTracking()
            .OrderBy(h => h.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Hospedes
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        _db.Hospedes.Add(hospede);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        _db.Hospedes.Update(hospede);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var hospede = await _db.Hospedes.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        if (hospede is null)
            return;

        _db.Hospedes.Remove(hospede);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
