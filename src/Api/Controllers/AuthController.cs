using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRecuperacaoSenhaService _recuperacaoSenhaService;

    public AuthController(IAuthService authService, IRecuperacaoSenhaService recuperacaoSenhaService)
    {
        _authService = authService;
        _recuperacaoSenhaService = recuperacaoSenhaService;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<UsuarioTokenDto>> Registrar([FromBody] UsuarioCreateDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _authService.ResgistrarAsync(dto.Nome, dto.Email, dto.Senha, "Gerente");
        var token = _authService.GerarToken(usuario);

        var resposta = new UsuarioTokenDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Token = token
        };

        return Ok(resposta);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioTokenDto>> Login([FromBody] UsuarioLoginDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _authService.AutenticarAsync(dto.Email, dto.Senha);
        var token = _authService.GerarToken(usuario);

        var resposta = new UsuarioTokenDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Token = token
        };

        return Ok(resposta);
    }

    [HttpPost("esqueci-senha")]
    public async Task<ActionResult<MensagemDto>> EsqueciSenha(
        [FromBody] EsqueciSenhaDto dto,
        CancellationToken cancellationToken = default)
    {
        await _recuperacaoSenhaService.SolicitarPorEmailAsync(dto.Email, cancellationToken);

        return Ok(new MensagemDto
        {
            Message = "Se o e-mail estiver cadastrado, você receberá um link para redefinir sua senha."
        });
    }

    [HttpPost("redefinir-senha")]
    public async Task<ActionResult<MensagemDto>> RedefinirSenha(
        [FromBody] RedefinirSenhaDto dto,
        CancellationToken cancellationToken = default)
    {
        await _recuperacaoSenhaService.RedefinirAsync(dto.Token, dto.SenhaNova, cancellationToken);

        return Ok(new MensagemDto { Message = "Senha redefinida com sucesso." });
    }
}
