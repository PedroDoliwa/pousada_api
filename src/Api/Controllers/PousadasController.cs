using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PousadasController : ControllerBase
{
    private readonly IPousadaService _pousadaService;

    public PousadasController(IPousadaService pousadaService)
    {
        _pousadaService = pousadaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PousadaReadDto>>> ListarPousadas(CancellationToken cancellationToken)
    {
        var pousadas = await _pousadaService.ListarAsync(cancellationToken);
        var result = pousadas.Select(p => new PousadaReadDto
        {
            Id = p.Id,
            UsuarioId = p.UsuarioId,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Endereco = p.Endereco,
            Telefone = p.Telefone,
            Email = p.Email,
            Ativa = p.Ativa
        }).ToList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PousadaReadDto>> ObterPousada(int id, CancellationToken cancellationToken)
    {
        var pousada = await _pousadaService.ObterPorIdAsync(id, cancellationToken);
        if (pousada == null)
            return NotFound(new { message = "Pousada não encontrada" });

        var result = new PousadaReadDto
        {
            Id = pousada.Id,
            UsuarioId = pousada.UsuarioId,
            Nome = pousada.Nome,
            Descricao = pousada.Descricao,
            Endereco = pousada.Endereco,
            Telefone = pousada.Telefone,
            Email = pousada.Email,
            Ativa = pousada.Ativa
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PousadaReadDto>> CriarPousada([FromBody] PousadaCreateDto dto, CancellationToken cancellationToken)
    {
        var pousada = new Pousada
        {
            UsuarioId = dto.UsuarioId,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Endereco = dto.Endereco,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Ativa = true
        };

        var criada = await _pousadaService.CriarAsync(pousada, cancellationToken);

        var result = new PousadaReadDto
        {
            Id = criada.Id,
            UsuarioId = criada.UsuarioId,
            Nome = criada.Nome,
            Descricao = criada.Descricao,
            Endereco = criada.Endereco,
            Telefone = criada.Telefone,
            Email = criada.Email,
            Ativa = criada.Ativa
        };
        return CreatedAtAction(nameof(ObterPousada), new { id = criada.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPousada(int id, [FromBody] PousadaUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var pousada = await _pousadaService.ObterPorIdAsync(id, cancellationToken);
        if (pousada == null)
            return NotFound(new { message = "Pousada não encontrada" });

        pousada.Nome = dto.Nome;
        pousada.Descricao = dto.Descricao;
        pousada.Endereco = dto.Endereco;
        pousada.Telefone = dto.Telefone;
        pousada.Email = dto.Email;

        await _pousadaService.AtualizarAsync(pousada, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverPousada(int id, CancellationToken cancellationToken)
    {
        var pousada = await _pousadaService.ObterPorIdAsync(id, cancellationToken);
        if (pousada == null)
            return NotFound(new { message = "Pousada não encontrada" });

        await _pousadaService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
