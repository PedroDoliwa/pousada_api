using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Infrastructure.Data;

namespace PousadaApi.Application.Services;

public class QuartoService : IQuartoService
{
    private readonly PousadaDbContext _dbContext;

    public QuartoService(PousadaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Quartos
            .Include(q => q.Pousada)
            .AsNoTracking()
            .AsQueryable();

        if (pousadaId.HasValue)
        {
            query = query.Where(q => q.PousadaId == pousadaId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Quartos
            .Include(q => q.Pousada)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<Quarto> CriarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaExistenteAsync(quarto.PousadaId, cancellationToken);

        _dbContext.Quartos.Add(quarto);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return quarto;
    }

    public async Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaExistenteAsync(quarto.PousadaId, cancellationToken);

        _dbContext.Quartos.Update(quarto);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var quarto = await _dbContext.Quartos.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (quarto == null)
        {
            return;
        }

        _dbContext.Quartos.Remove(quarto);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidarPousadaExistenteAsync(int pousadaId, CancellationToken cancellationToken)
    {
        var existe = await _dbContext.Pousadas.AnyAsync(p => p.Id == pousadaId, cancellationToken);
        if (!existe)
        {
            throw new InvalidOperationException("Pousada informada não foi encontrada.");
        }
    }
}
