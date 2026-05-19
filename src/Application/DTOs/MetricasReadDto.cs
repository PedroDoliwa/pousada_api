namespace PousadaApi.Application.DTOs;

public class MetricasReadDto
{
    public int PousadaId { get; set; }
    public DateTime De { get; set; }
    public DateTime Ate { get; set; }
    public int TotalQuartos { get; set; }
    public int TotalReservas { get; set; }
    public decimal TaxaOcupacaoPercentual { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public int HospedesUnicos { get; set; }
}
