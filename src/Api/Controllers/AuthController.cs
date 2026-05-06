using Microsoft.AspNetCore.Mvc;
using PousadaApi.Api.Dtos;
using PousadaApi.Application.Services;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<UsuarioTokenDto>> Registrar([FromBody] UsuarioCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _authService.ResgistrarAsync(dto.Nome, dto.Email, dto.Senha, dto.Perfil);
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao registrar usuário", erro = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UsuarioTokenDto>> Login([FromBody] UsuarioLoginDto dto, CancellationToken cancellationToken = default)
    {
        try
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = "Erro ao fazer login", erro = ex.Message });
        }
    }
}
