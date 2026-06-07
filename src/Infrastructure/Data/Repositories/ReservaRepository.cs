using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Constants;
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

    public async Task<IEnumerable<Reserva>> ListarPorUsuarioAsync(int usuarioId, int? pousadaId, CancellationToken cancellationToken = default)
    {
        var query = _db.Reservas
            .Include(r => r.Quarto)
            .ThenInclude(q => q!.Pousada)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .Where(r => r.Quarto != null && r.Quarto.Pousada != null && r.Quarto.Pousada.UsuarioId == usuarioId);

        if (pousadaId.HasValue)
            query = query.Where(r => r.Quarto!.PousadaId == pousadaId.Value);

        return await query.OrderByDescending(r => r.DataEntrada).ToListAsync(cancellationToken);
    }

    public async Task<Reserva?> ObterPorIdEUsuarioAsync(int id, int usuarioId, CancellationToken cancellationToken = default)
    {
        return await _db.Reservas
            .Include(r => r.Quarto)
            .ThenInclude(q => q!.Pousada)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.Id == id && r.Quarto != null && r.Quarto.Pousada != null && r.Quarto.Pousada.UsuarioId == usuarioId,
                cancellationToken);
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
        var query = _db.Reservas.Where(r => r.QuartoId == quartoId && r.Status != ReservaStatus.Cancelada);

        if (ignorarReservaId.HasValue)
            query = query.Where(r => r.Id != ignorarReservaId.Value);

        return query.AnyAsync(
            r => r.DataEntrada < dataSaida && r.DataSaida > dataEntrada,
            cancellationToken);
    }

    public async Task<IEnumerable<Reserva>> ListarOcupacaoPorPousadaAsync(
        int usuarioId,
        int pousadaId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default)
    {
        return await _db.Reservas
            .Include(r => r.Quarto)
            .ThenInclude(q => q!.Pousada)
            .Include(r => r.Hospede)
            .AsNoTracking()
            .Where(r =>
                r.Quarto != null &&
                r.Quarto.Pousada != null &&
                r.Quarto.Pousada.UsuarioId == usuarioId &&
                r.Quarto.PousadaId == pousadaId &&
                r.Status != ReservaStatus.Cancelada &&
                r.DataEntrada < ate &&
                r.DataSaida > de)
            .OrderBy(r => r.DataEntrada)
            .ToListAsync(cancellationToken);
    }

    public Task<Reserva?> ObterPorUidExternoAsync(int quartoId, string uidExterno, CancellationToken cancellationToken = default)
    {
        return _db.Reservas.FirstOrDefaultAsync(
            r => r.QuartoId == quartoId && r.UidExterno == uidExterno,
            cancellationToken);
    }

    public async Task<IEnumerable<Reserva>> ListarAtivasPorQuartoAsync(int quartoId, CancellationToken cancellationToken = default)
    {
        return await _db.Reservas
            .AsNoTracking()
            .Where(r => r.QuartoId == quartoId && r.Status != ReservaStatus.Cancelada)
            .OrderBy(r => r.DataEntrada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reserva>> ListarPorCalendarioExternoAsync(int calendarioExternoId, CancellationToken cancellationToken = default)
    {
        return await _db.Reservas
            .Where(r => r.CalendarioExternoId == calendarioExternoId && r.Status != ReservaStatus.Cancelada)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reserva>> ListarConfirmadasPorPousadaNoPeriodoAsync(
        int usuarioId,
        int pousadaId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default)
    {
        return await _db.Reservas
            .Include(r => r.Quarto)
            .ThenInclude(q => q!.Pousada)
            .AsNoTracking()
            .Where(r =>
                r.Quarto != null &&
                r.Quarto.Pousada != null &&
                r.Quarto.Pousada.UsuarioId == usuarioId &&
                r.Quarto.PousadaId == pousadaId &&
                r.Status == ReservaStatus.Confirmada &&
                r.DataEntrada < ate &&
                r.DataSaida > de)
            .ToListAsync(cancellationToken);
    }
}
