using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class UsuarioRecuperacaoSenhaRepository : IUsuarioRecuperacaoSenhaRepository
{
    private readonly PousadaDbContext _db;

    public UsuarioRecuperacaoSenhaRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public async Task AdicionarAsync(UsuarioRecuperacaoSenha recuperacao, CancellationToken cancellationToken = default)
    {
        _db.UsuarioRecuperacaoSenhas.Add(recuperacao);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<UsuarioRecuperacaoSenha?> ObterValidoPorTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var agora = DateTime.UtcNow;
        return _db.UsuarioRecuperacaoSenhas
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(
                r => r.TokenHash == tokenHash && r.UsadoEm == null && r.ExpiraEm > agora,
                cancellationToken);
    }

    public async Task MarcarComoUsadoAsync(UsuarioRecuperacaoSenha recuperacao, CancellationToken cancellationToken = default)
    {
        recuperacao.UsadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
