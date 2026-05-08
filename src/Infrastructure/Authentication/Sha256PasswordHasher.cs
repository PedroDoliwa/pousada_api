using System.Security.Cryptography;
using System.Text;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Infrastructure.Authentication;

public sealed class Sha256PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool Verify(string password, string hash)
    {
        return Hash(password) == hash;
    }
}
