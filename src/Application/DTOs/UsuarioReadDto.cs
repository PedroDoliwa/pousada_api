namespace PousadaApi.Application.DTOs;

public class UsuarioReadDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public bool TemFoto { get; set; }
}
