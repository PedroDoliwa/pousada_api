using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HospedesController : ControllerBase
{
    private readonly IHospedeService _hospedeService;

    public HospedesController(IHospedeService hospedeService)
    {
        _hospedeService = hospedeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HospedeReadDto>>> ListarHospedes(CancellationToken cancellationToken)
    {
        var hospedes = await _hospedeService.ListarAsync(cancellationToken);
        var result = hospedes.Select(h => new HospedeReadDto
        {
            Id = h.Id,
            Nome = h.Nome,
            Telefone = h.Telefone,
            Email = h.Email,
            Documento = h.Documento
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HospedeReadDto>> ObterHospede(int id, CancellationToken cancellationToken)
    {
        var hospede = await _hospedeService.ObterPorIdAsync(id, cancellationToken);
        if (hospede == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        var result = new HospedeReadDto
        {
            Id = hospede.Id,
            Nome = hospede.Nome,
            Telefone = hospede.Telefone,
            Email = hospede.Email,
            Documento = hospede.Documento
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<HospedeReadDto>> CriarHospede([FromBody] HospedeCreateDto dto, CancellationToken cancellationToken)
    {
        var hospede = new Hospede
        {
            Nome = dto.Nome,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Documento = dto.Documento
        };

        var criado = await _hospedeService.CriarAsync(hospede, cancellationToken);

        var result = new HospedeReadDto
        {
            Id = criado.Id,
            Nome = criado.Nome,
            Telefone = criado.Telefone,
            Email = criado.Email,
            Documento = criado.Documento
        };

        return CreatedAtAction(nameof(ObterHospede), new { id = criado.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarHospede(int id, [FromBody] HospedeUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var hospede = await _hospedeService.ObterPorIdAsync(id, cancellationToken);
        if (hospede == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        hospede.Nome = dto.Nome;
        hospede.Telefone = dto.Telefone;
        hospede.Email = dto.Email;
        hospede.Documento = dto.Documento;

        await _hospedeService.AtualizarAsync(hospede, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverHospede(int id, CancellationToken cancellationToken)
    {
        var hospede = await _hospedeService.ObterPorIdAsync(id, cancellationToken);
        if (hospede == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        await _hospedeService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
