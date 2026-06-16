using System.Security.Cryptography;
using System.Text;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Infrastructure.Authentication;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    // Custo de trabalho do BCrypt. Quanto maior, mais lento (e mais seguro contra
    // força bruta). 12 ~= 250ms por verificação em hardware atual: bom equilíbrio.
    private const int WorkFactor = 12;

    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash)
    {
        // Sem senha ou sem hash armazenado não há o que validar (ex.: seed placeholder).
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        // Hashes BCrypt sempre começam com "$2"; hashes antigos (SHA256 em Base64) não.
        // Mantemos a verificação legada para não travar usuários criados antes da
        // migração. Esses usuários passam a usar BCrypt assim que redefinirem a senha.
        if (hash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(password, hash);

        return VerificarLegadoSha256(password, hash);
    }

    private static bool VerificarLegadoSha256(string password, string hash)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var legacyHash = Convert.ToBase64String(hashedBytes);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(legacyHash),
            Encoding.UTF8.GetBytes(hash));
    }
}
