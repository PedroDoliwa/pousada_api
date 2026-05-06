namespace PousadaApi.Api.Dtos;

public class UsuarioCreateDto
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Perfil { get; set; } = "Gerente";
}
