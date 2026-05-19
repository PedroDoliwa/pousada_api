namespace PousadaApi.Application.DTOs;

public class QuartoReadDto
{
    public int Id { get; set; }
    public int PousadaId { get; set; }
    public string NumeroOuNome { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public decimal ValorDiaria { get; set; }
    public string Status { get; set; } = "Disponivel";
    public string TokenExportacao { get; set; } = string.Empty;
}
