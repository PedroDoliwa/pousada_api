namespace PousadaApi.Api.Dtos;

public class HospedeUpdateDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Documento { get; set; }
}