using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Models;

public sealed class ConsultaToolExecutionResult
{
    public string ResultJson { get; set; } = "{}";

    public ConsultaPeriodoDto? PeriodoConsultado { get; set; }
}
