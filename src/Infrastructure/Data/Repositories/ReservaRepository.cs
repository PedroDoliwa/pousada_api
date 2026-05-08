using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class ReservaRepository : IReservaRepository
{
    private readonly PousadaDbContext _db;

    public ReservaRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Reserva>> ListarComRelacionamentosAsync(int? pousadaId, CancellationToken cancellationToken = default)
    {
        var query = _db.Reservas
            .Include(r => r.Quarto)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .AsQueryable();

        if (pousadaId.HasValue)
            query = query.Where(r => r.Quarto != null && r.Quarto.PousadaId == pousadaId.Value);

        return await query.OrderByDescending(r => r.DataEntrada).ToListAsync(cancellationToken);
    }

    public Task<Reserva?> ObterPorIdComRelacionamentosAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Reservas
            .Include(r => r.Quarto)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<Reserva?> ObterPorIdRastreadoAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Reservas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        _db.Reservas.Add(reserva);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        _db.Reservas.Update(reserva);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExisteSobreposicaoNoQuartoAsync(
        int quartoId,
        DateTime dataEntrada,
        DateTime dataSaida,
        int? ignorarReservaId,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Reservas.Where(r => r.QuartoId == quartoId && r.Status != "Cancelada");

        if (ignorarReservaId.HasValue)
            query = query.Where(r => r.Id != ignorarReservaId.Value);

        return query.AnyAsync(
            r => r.DataEntrada < dataSaida && r.DataSaida > dataEntrada,
            cancellationToken);
    }
}
