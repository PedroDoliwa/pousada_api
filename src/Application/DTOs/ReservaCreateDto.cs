namespace PousadaApi.Application.DTOs;

public class ReservaCreateDto
{
    public int QuartoId { get; set; }
    public int HospedeId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public string? Observacoes { get; set; }
}
