using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secretKey = JwtSecretKey.ObterValidada(configuration);
    }

    public string Generate(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("email", usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim("perfil", usuario.Perfil)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
