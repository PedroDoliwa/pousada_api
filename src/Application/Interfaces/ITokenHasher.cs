namespace PousadaApi.Application.Interfaces;

// Hash determinístico para tokens (ex.: recuperação de senha), permitindo buscar
// pelo hash no banco. BCrypt (IPasswordHasher) não serve aqui pois o salt é aleatório.
public interface ITokenHasher
{
    string Hash(string token);
}
