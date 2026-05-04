namespace PousadaApi.Api.Dtos;

public class QuartoUpdateDto
{
    public int Id { get; set; }
    public string NumeroOuNome { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public decimal ValorDiaria { get; set; }
    public string Status { get; set; } = "Disponivel";
}
