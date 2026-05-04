using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.Services;
using PousadaApi.Api.Dtos;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuartosController : ControllerBase
{
    private readonly IQuartoService _quartoService;

    public QuartosController(IQuartoService quartoService)
    {
        _quartoService = quartoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuartoReadDto>>> ListarQuartos([FromQuery] int? pousadaId, CancellationToken cancellationToken)
    {
        try
        {
            var quartos = await _quartoService.ListarAsync(pousadaId, cancellationToken);
            var result = quartos.Select(q => new QuartoReadDto
            {
                Id = q.Id,
                PousadaId = q.PousadaId,
                NumeroOuNome = q.NumeroOuNome,
                Capacidade = q.Capacidade,
                ValorDiaria = q.ValorDiaria,
                Status = q.Status
            }).ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuartoReadDto>> ObterQuarto(int id, CancellationToken cancellationToken)
    {
        try
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
                Status = quarto.Status
            };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuartoReadDto>> CriarQuarto([FromBody] QuartoCreateDto dto, CancellationToken cancellationToken)
    {
        try
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
                Status = criado.Status
            };
            return CreatedAtAction(nameof(ObterQuarto), new { id = criado.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarQuarto(int id, [FromBody] QuartoUpdateDto dto, CancellationToken cancellationToken)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverQuarto(int id, CancellationToken cancellationToken)
    {
        try
        {
            var quarto = await _quartoService.ObterPorIdAsync(id, cancellationToken);
            if (quarto == null)
                return NotFound(new { message = "Quarto não encontrado" });

            await _quartoService.RemoverAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
