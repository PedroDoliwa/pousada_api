namespace PousadaApi.Application.DTOs;

public class HospedeReadDto
{
    public int Id { get; set; }
    public int PousadaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Documento { get; set; }
}
