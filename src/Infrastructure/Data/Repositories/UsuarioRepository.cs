using Microsoft.EntityFrameworkCore;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Infrastructure.Data.Context;

namespace PousadaApi.Infrastructure.Data.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly PousadaDbContext _db;

    public UsuarioRepository(PousadaDbContext db)
    {
        _db = db;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<bool> ExistePorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Usuarios.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
