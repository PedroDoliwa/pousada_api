using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IAuthService
{
    Task<Usuario> ResgistrarAsync(string nome, string email, string senha, string perfil = "Gerente");
    Task<Usuario> AutenticarAsync(string email, string senha);
    string GerarToken(Usuario usuario);
}
