using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetricasController : ControllerBase
{
    private readonly IMetricasService _metricasService;

    public MetricasController(IMetricasService metricasService)
    {
        _metricasService = metricasService;
    }

    [HttpGet]
    public async Task<ActionResult<MetricasReadDto>> Obter(
        [FromQuery] int pousadaId,
        [FromQuery] DateTime de,
        [FromQuery] DateTime ate,
        CancellationToken cancellationToken)
    {
        var metricas = await _metricasService.ObterAsync(pousadaId, de, ate, cancellationToken);
        return Ok(metricas);
    }
}
