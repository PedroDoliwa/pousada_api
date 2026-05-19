using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendariosController : ControllerBase
{
    private readonly ICalendarioExternoService _calendarioService;

    public CalendariosController(ICalendarioExternoService calendarioService)
    {
        _calendarioService = calendarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarioExternoReadDto>>> Listar([FromQuery] int quartoId, CancellationToken cancellationToken)
    {
        var itens = await _calendarioService.ListarAsync(quartoId, cancellationToken);
        return Ok(itens);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CalendarioExternoReadDto>> Obter(int id, CancellationToken cancellationToken)
    {
        var cal = await _calendarioService.ObterPorIdAsync(id, cancellationToken);
        if (cal is null)
            return NotFound(new { message = "Calendário não encontrado" });
        return Ok(cal);
    }

    [HttpPost]
    public async Task<ActionResult<CalendarioExternoReadDto>> Criar([FromBody] CalendarioExternoCreateDto dto, CancellationToken cancellationToken)
    {
        var criado = await _calendarioService.CriarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CalendarioExternoUpdateDto dto, CancellationToken cancellationToken)
    {
        await _calendarioService.AtualizarAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _calendarioService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/sincronizar")]
    public async Task<ActionResult<CalendarioSyncResultDto>> Sincronizar(int id, CancellationToken cancellationToken)
    {
        var resultado = await _calendarioService.SincronizarAsync(id, cancellationToken);
        return Ok(resultado);
    }
}
