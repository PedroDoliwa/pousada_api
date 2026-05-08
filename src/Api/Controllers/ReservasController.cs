using Microsoft.AspNetCore.Mvc;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservaReadDto>>> ListarReservas([FromQuery] int? pousadaId, CancellationToken cancellationToken)
    {
        var reservas = await _reservaService.ListarAsync(pousadaId, cancellationToken);
        var result = reservas.Select(r => new ReservaReadDto
        {
            Id = r.Id,
            QuartoId = r.QuartoId,
            HospedeId = r.HospedeId,
            DataEntrada = r.DataEntrada,
            DataSaida = r.DataSaida,
            Status = r.Status,
            ValorTotal = r.ValorTotal,
            Observacoes = r.Observacoes
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservaReadDto>> ObterReserva(int id, CancellationToken cancellationToken)
    {
        var reserva = await _reservaService.ObterPorIdAsync(id, cancellationToken);
        if (reserva == null)
            return NotFound(new { message = "Reserva não encontrada" });

        var result = new ReservaReadDto
        {
            Id = reserva.Id,
            QuartoId = reserva.QuartoId,
            HospedeId = reserva.HospedeId,
            DataEntrada = reserva.DataEntrada,
            DataSaida = reserva.DataSaida,
            Status = reserva.Status,
            ValorTotal = reserva.ValorTotal,
            Observacoes = reserva.Observacoes
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReservaReadDto>> CriarReserva([FromBody] ReservaCreateDto dto, CancellationToken cancellationToken)
    {
        var reserva = new Reserva
        {
            QuartoId = dto.QuartoId,
            HospedeId = dto.HospedeId,
            DataEntrada = dto.DataEntrada,
            DataSaida = dto.DataSaida,
            Observacoes = dto.Observacoes
        };

        var criada = await _reservaService.CriarAsync(reserva, cancellationToken);

        var result = new ReservaReadDto
        {
            Id = criada.Id,
            QuartoId = criada.QuartoId,
            HospedeId = criada.HospedeId,
            DataEntrada = criada.DataEntrada,
            DataSaida = criada.DataSaida,
            Status = criada.Status,
            ValorTotal = criada.ValorTotal,
            Observacoes = criada.Observacoes
        };

        return CreatedAtAction(nameof(ObterReserva), new { id = criada.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarReserva(int id, [FromBody] ReservaUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID não corresponde" });

        var reserva = await _reservaService.ObterPorIdAsync(id, cancellationToken);
        if (reserva == null)
            return NotFound(new { message = "Reserva não encontrada" });

        reserva.QuartoId = dto.QuartoId;
        reserva.HospedeId = dto.HospedeId;
        reserva.DataEntrada = dto.DataEntrada;
        reserva.DataSaida = dto.DataSaida;
        if (!string.IsNullOrEmpty(dto.Status)) reserva.Status = dto.Status;
        reserva.Observacoes = dto.Observacoes;

        await _reservaService.AtualizarAsync(reserva, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarReserva(int id, CancellationToken cancellationToken)
    {
        var reserva = await _reservaService.ObterPorIdAsync(id, cancellationToken);
        if (reserva == null)
            return NotFound(new { message = "Reserva não encontrada" });

        await _reservaService.CancelarAsync(id, cancellationToken);
        return NoContent();
    }
}
