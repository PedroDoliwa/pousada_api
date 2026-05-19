namespace PousadaApi.Application.DTOs;

public class IcalEventoDto
{
    public string Uid { get; set; } = string.Empty;
    public string? Titulo { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Cancelado { get; set; }
}
