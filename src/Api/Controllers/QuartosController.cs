using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuartosController : ControllerBase
{
    private readonly IQuartoService _quartoService;
    private readonly IIcalExportService _icalExportService;

    public QuartosController(IQuartoService quartoService, IIcalExportService icalExportService)
    {
        _quartoService = quartoService;
        _icalExportService = icalExportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuartoReadDto>>> ListarQuartos([FromQuery] int? pousadaId, CancellationToken cancellationToken)
    {
        var quartos = await _quartoService.ListarAsync(pousadaId, cancellationToken);
        var result = quartos.Select(q => new QuartoReadDto
        {
            Id = q.Id,
            PousadaId = q.PousadaId,
            NumeroOuNome = q.NumeroOuNome,
            Capacidade = q.Capacidade,
            ValorDiaria = q.ValorDiaria,
            Status = q.Status,
            TokenExportacao = q.TokenExportacao
        }).ToList();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}/calendario.ics")]
    public async Task<IActionResult> ExportarCalendario(int id, [FromQuery] string? token, CancellationToken cancellationToken)
    {
        string ics;
        if (!string.IsNullOrWhiteSpace(token))
        {
            ics = await _icalExportService.GerarCalendarioPorTokenAsync(token, cancellationToken);
        }
        else if (User.Identity?.IsAuthenticated == true)
        {
            ics = await _icalExportService.GerarCalendarioQuartoAsync(id, cancellationToken);
        }
        else
        {
            return Unauthorized(new { message = "Informe o parâmetro token ou autentique-se com Bearer JWT." });
        }

        return Content(ics, "text/calendar");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuartoReadDto>> ObterQuarto(int id, CancellationToken cancellationToken)
    {
        var quarto = await _quartoService.ObterPorIdAsync(id, cancellationToken);
        if (quarto == null)
            return NotFound(new { message = "Quarto não encontrado" });

        var result = new QuartoReadDto
        {
            Id = quarto.Id,
            PousadaId = quarto.PousadaId,
            NumeroOuNome = quarto.NumeroOuNome,
            Capacidade = quarto.Capacidade,
            ValorDiaria = quarto.ValorDiaria,
            Status = quarto.Status,
            TokenExportacao = quarto.TokenExportacao
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<QuartoReadDto>> CriarQuarto([FromBody] QuartoCreateDto dto, CancellationToken cancellationToken)
    {
        var quarto = new Quarto
        {
            PousadaId = dto.PousadaId,
            NumeroOuNome = dto.NumeroOuNome,
            Capacidade = dto.Capacidade,
            ValorDiaria = dto.ValorDiaria,
            Status = "Disponivel"
        };

        var criado = await _quartoService.CriarAsync(quarto, cancellationToken);

        var result = new QuartoReadDto
        {
            Id = criado.Id,
            PousadaId = criado.PousadaId,
            NumeroOuNome = criado.NumeroOuNome,
            Capacidade = criado.Capacidade,
            ValorDiaria = criado.ValorDiaria,
            Status = criado.Status,
            TokenExportacao = criado.TokenExportacao
        };
        return CreatedAtAction(nameof(ObterQuarto), new { id = criado.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarQuarto(int id, [FromBody] QuartoUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var quarto = await _quartoService.ObterPorIdAsync(id, cancellationToken);
        if (quarto == null)
            return NotFound(new { message = "Quarto não encontrado" });

        quarto.NumeroOuNome = dto.NumeroOuNome;
        quarto.Capacidade = dto.Capacidade;
        quarto.ValorDiaria = dto.ValorDiaria;
        quarto.Status = dto.Status;

        await _quartoService.AtualizarAsync(quarto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverQuarto(int id, CancellationToken cancellationToken)
    {
        var quarto = await _quartoService.ObterPorIdAsync(id, cancellationToken);
        if (quarto == null)
            return NotFound(new { message = "Quarto não encontrado" });

        await _quartoService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
