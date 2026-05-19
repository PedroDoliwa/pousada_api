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

    public async Task<IEnumerable<Hospede>> ListarPorUsuarioAsync(int usuarioId, int? pousadaId, CancellationToken cancellationToken = default)
    {
        var query = _db.Hospedes
            .Include(h => h.Pousada)
            .AsNoTracking()
            .Where(h => h.Pousada != null && h.Pousada.UsuarioId == usuarioId);

        if (pousadaId.HasValue)
            query = query.Where(h => h.PousadaId == pousadaId.Value);

        return await query.OrderBy(h => h.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Hospede?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default)
    {
        return await _db.Hospedes
            .Include(h => h.Pousada)
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id && h.Pousada != null && h.Pousada.UsuarioId == usuarioId, cancellationToken);
    }

    public Task<bool> PousadaPertenceAoUsuarioAsync(int pousadaId, int usuarioId, CancellationToken cancellationToken = default)
    {
        return _db.Pousadas.AnyAsync(p => p.Id == pousadaId && p.UsuarioId == usuarioId, cancellationToken);
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
