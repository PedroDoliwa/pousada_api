namespace PousadaApi.Application.DTOs;

public class QuartoCreateDto
{
    public int PousadaId { get; set; }
    public string NumeroOuNome { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public decimal ValorDiaria { get; set; }
}
