namespace PousadaApi.Api.Dtos;

public class ReservaUpdateDto
{
    public int Id { get; set; }
    public int QuartoId { get; set; }
    public int HospedeId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public string? Status { get; set; }
    public string? Observacoes { get; set; }
}
