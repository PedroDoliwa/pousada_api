using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IRecuperacaoSenhaService _recuperacaoSenhaService;
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public UsuarioController(
        IUsuarioService usuarioService,
        IRecuperacaoSenhaService recuperacaoSenhaService,
        IAuthService authService,
        ICurrentUserService currentUser)
    {
        _usuarioService = usuarioService;
        _recuperacaoSenhaService = recuperacaoSenhaService;
        _authService = authService;
        _currentUser = currentUser;
    }

    [HttpGet("perfil")]
    public async Task<ActionResult<UsuarioReadDto>> ObterPerfil(CancellationToken cancellationToken)
    {
        var perfil = await _usuarioService.ObterPerfilAsync(_currentUser.UserId, cancellationToken);
        return Ok(perfil);
    }

    [HttpPut("perfil")]
    public async Task<ActionResult<UsuarioPerfilAtualizadoDto>> AtualizarPerfil(
        [FromBody] UsuarioUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.AtualizarNomeAsync(_currentUser.UserId, dto.Nome, cancellationToken);
        var token = _authService.GerarToken(usuario);

        return Ok(new UsuarioPerfilAtualizadoDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            TemFoto = usuario.Foto is { Length: > 0 },
            Token = token
        });
    }

    [HttpPut("senha")]
    public async Task<ActionResult<MensagemDto>> AlterarSenha(
        [FromBody] UsuarioSenhaUpdateDto dto,
        CancellationToken cancellationToken)
    {
        await _usuarioService.AlterarSenhaAsync(
            _currentUser.UserId,
            dto.SenhaAtual,
            dto.SenhaNova,
            cancellationToken);

        return Ok(new MensagemDto { Message = "Senha alterada com sucesso." });
    }

    [HttpPost("solicitar-redefinicao-senha")]
    public async Task<ActionResult<MensagemDto>> SolicitarRedefinicaoSenha(CancellationToken cancellationToken)
    {
        await _recuperacaoSenhaService.SolicitarPorUsuarioAsync(_currentUser.UserId, cancellationToken);
        var perfil = await _usuarioService.ObterPerfilAsync(_currentUser.UserId, cancellationToken);

        return Ok(new MensagemDto
        {
            Message = $"Enviamos um link de redefinição para {perfil.Email}."
        });
    }

    [HttpPost("foto")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<ActionResult<MensagemDto>> SalvarFoto(IFormFile arquivo, CancellationToken cancellationToken)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { message = "Arquivo de foto é obrigatório." });

        await using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream, cancellationToken);

        await _usuarioService.SalvarFotoAsync(
            _currentUser.UserId,
            stream.ToArray(),
            arquivo.ContentType,
            cancellationToken);

        return Ok(new MensagemDto { Message = "Foto de perfil atualizada com sucesso." });
    }

    [HttpGet("foto")]
    public async Task<IActionResult> ObterFoto(CancellationToken cancellationToken)
    {
        var (bytes, contentType) = await _usuarioService.ObterFotoAsync(_currentUser.UserId, cancellationToken);
        if (bytes is null || contentType is null)
            return NotFound(new { message = "Foto de perfil não encontrada." });

        return File(bytes, contentType);
    }

    [HttpDelete("foto")]
    public async Task<ActionResult<MensagemDto>> RemoverFoto(CancellationToken cancellationToken)
    {
        await _usuarioService.RemoverFotoAsync(_currentUser.UserId, cancellationToken);
        return Ok(new MensagemDto { Message = "Foto de perfil removida com sucesso." });
    }
}
