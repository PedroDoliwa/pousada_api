using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Infrastructure.Data;

namespace PousadaApi.Application.Services;

public class PousadaService : IPousadaService
{
    private readonly PousadaDbContext _dbContext;

    public PousadaService(PousadaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pousadas
            .Include(p => p.Quartos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Pousadas
            .Include(p => p.Quartos)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        _dbContext.Pousadas.Add(pousada);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return pousada;
    }

    public async Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        _dbContext.Pousadas.Update(pousada);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var pousada = await _dbContext.Pousadas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pousada == null)
        {
            return;
        }

        _dbContext.Pousadas.Remove(pousada);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
