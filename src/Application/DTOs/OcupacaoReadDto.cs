namespace PousadaApi.Application.DTOs;

public class OcupacaoReadDto
{
    public int ReservaId { get; set; }
    public int QuartoId { get; set; }
    public string QuartoNumeroOuNome { get; set; } = string.Empty;
    public int HospedeId { get; set; }
    public string HospedeNome { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Origem { get; set; } = "Manual";
    public string? TituloExterno { get; set; }
    public string? Observacoes { get; set; }
}
