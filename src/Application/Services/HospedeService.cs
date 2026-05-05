using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Infrastructure.Data;

namespace PousadaApi.Application.Services;

public class HospedeService : IHospedeService
{
    private readonly PousadaDbContext _dbContext;

    public HospedeService(PousadaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hospedes
            .AsNoTracking()
            .OrderBy(h => h.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Hospedes
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        _dbContext.Hospedes.Add(hospede);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return hospede;
    }

    public async Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        _dbContext.Hospedes.Update(hospede);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var hospede = await _dbContext.Hospedes.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        if (hospede == null)
        {
            return;
        }

        _dbContext.Hospedes.Remove(hospede);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
