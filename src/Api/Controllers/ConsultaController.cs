using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Controllers;

/// <summary>
/// Consulta inteligente em linguagem natural sobre dados da pousada.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultaController : ControllerBase
{
    private readonly IConsultaInteligenteService _consultaService;

    public ConsultaController(IConsultaInteligenteService consultaService)
    {
        _consultaService = consultaService;
    }

    /// <summary>
    /// Envia uma pergunta em linguagem natural sobre métricas, reservas, quartos ou hóspedes da pousada.
    /// </summary>
    /// <param name="request">Pousada, pergunta e histórico opcional da conversa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <response code="200">Resposta gerada pela IA com base nos dados do sistema.</response>
    /// <response code="400">Requisição inválida ou parâmetros incorretos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pousada não encontrada ou sem acesso.</response>
    /// <response code="503">Serviço de consulta (OpenAI) temporariamente indisponível.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConsultaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ConsultaResponseDto>> Consultar(
        [FromBody] ConsultaRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resposta = await _consultaService.ConsultarAsync(request, cancellationToken);
        return Ok(resposta);
    }
}
