using PousadaApi.Domain.Entities;
using PousadaApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PousadaApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly PousadaDbContext _context;
    private readonly string _secretKey;

    public AuthService(PousadaDbContext context, IConfiguration configuration)
    {
        _context = context;
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada");
    }

    public async Task<Usuario> ResgistrarAsync(string nome, string email, string senha, string perfil = "Gerente")
    {
        // Verificar se email já existe
        var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuarioExistente != null)
            throw new InvalidOperationException("Email já cadastrado");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = HashSenha(senha),
            Perfil = perfil
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario> AutenticarAsync(string email, string senha)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        
        if (usuario == null || !VerificarSenha(senha, usuario.SenhaHash))
            throw new InvalidOperationException("Email ou senha inválidos");

        return usuario;
    }

    public string GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("id", usuario.Id.ToString()),
                new System.Security.Claims.Claim("email", usuario.Email),
                new System.Security.Claims.Claim("nome", usuario.Nome),
                new System.Security.Claims.Claim("perfil", usuario.Perfil)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string HashSenha(string senha)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerificarSenha(string senha, string hash)
    {
        var hashDaSenha = HashSenha(senha);
        return hashDaSenha == hash;
    }
}
