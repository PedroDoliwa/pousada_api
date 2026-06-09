namespace PousadaApi.Application.DTOs;

public class ConsultaResponseDto
{
    public string Resposta { get; set; } = string.Empty;

    public List<string> FerramentasUsadas { get; set; } = [];

    public ConsultaPeriodoDto? PeriodoConsultado { get; set; }
}
