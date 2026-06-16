using System.Security.Cryptography;
using System.Text;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Infrastructure.Authentication;

// SHA-256 determinístico para tokens. Seguro pois o token tem 256 bits aleatórios
// (força bruta inviável, sem precisar do custo do BCrypt).
public sealed class Sha256TokenHasher : ITokenHasher
{
    public string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
