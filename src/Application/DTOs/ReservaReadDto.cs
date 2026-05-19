namespace PousadaApi.Application.DTOs;

public class ReservaReadDto
{
    public int Id { get; set; }
    public int QuartoId { get; set; }
    public int HospedeId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public string Origem { get; set; } = "Manual";
    public string? TituloExterno { get; set; }
}
