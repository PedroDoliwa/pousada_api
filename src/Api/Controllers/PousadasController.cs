using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        return Ok(pousadas.Select(MapearPousada).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PousadaReadDto>> ObterPousada(int id, CancellationToken cancellationToken)
    {
        var pousada = await _pousadaService.ObterPorIdAsync(id, cancellationToken);
        if (pousada == null)
            return NotFound(new { message = "Pousada não encontrada" });

        return Ok(MapearPousada(pousada));
    }

    [HttpPost]
    public async Task<ActionResult<PousadaReadDto>> CriarPousada([FromBody] PousadaCreateDto dto, CancellationToken cancellationToken)
    {
        var pousada = new Pousada
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Endereco = dto.Endereco,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Ativa = true
        };

        var criada = await _pousadaService.CriarAsync(pousada, cancellationToken);
        return CreatedAtAction(nameof(ObterPousada), new { id = criada.Id }, MapearPousada(criada));
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

    [HttpPost("{id}/foto")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<ActionResult<MensagemDto>> SalvarFoto(int id, IFormFile arquivo, CancellationToken cancellationToken)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { message = "Arquivo de foto é obrigatório." });

        await using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream, cancellationToken);

        await _pousadaService.SalvarFotoAsync(
            id,
            stream.ToArray(),
            arquivo.ContentType,
            cancellationToken);

        return Ok(new MensagemDto { Message = "Foto da pousada atualizada com sucesso." });
    }

    [HttpGet("{id}/foto")]
    public async Task<IActionResult> ObterFoto(int id, CancellationToken cancellationToken)
    {
        var (bytes, contentType) = await _pousadaService.ObterFotoAsync(id, cancellationToken);
        if (bytes is null || contentType is null)
            return NotFound(new { message = "Foto da pousada não encontrada." });

        return File(bytes, contentType);
    }

    [HttpDelete("{id}/foto")]
    public async Task<ActionResult<MensagemDto>> RemoverFoto(int id, CancellationToken cancellationToken)
    {
        await _pousadaService.RemoverFotoAsync(id, cancellationToken);
        return Ok(new MensagemDto { Message = "Foto da pousada removida com sucesso." });
    }

    private static PousadaReadDto MapearPousada(Pousada pousada) => new()
    {
        Id = pousada.Id,
        UsuarioId = pousada.UsuarioId,
        Nome = pousada.Nome,
        Descricao = pousada.Descricao,
        Endereco = pousada.Endereco,
        Telefone = pousada.Telefone,
        Email = pousada.Email,
        Ativa = pousada.Ativa,
        TemFoto = pousada.Foto is { Length: > 0 }
    };
}
