using Microsoft.Extensions.Configuration;

namespace PousadaApi.Infrastructure.Authentication;

public static class JwtSecretKey
{
    // HS256 exige uma chave de pelo menos 256 bits (32 bytes/caracteres).
    private const int TamanhoMinimo = 32;

    public static string ObterValidada(IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException(
                "JWT SecretKey não configurada. Defina a variável de ambiente 'Jwt__SecretKey' " +
                "(ou no arquivo .env local) com uma chave aleatória de pelo menos 32 caracteres.");

        if (secretKey.Length < TamanhoMinimo)
            throw new InvalidOperationException(
                $"JWT SecretKey muito curta ({secretKey.Length} caracteres). " +
                $"Use pelo menos {TamanhoMinimo} caracteres.");

        return secretKey;
    }
}
