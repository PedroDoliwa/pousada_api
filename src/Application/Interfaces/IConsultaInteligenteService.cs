using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Interfaces;

public interface IConsultaInteligenteService
{
    Task<ConsultaResponseDto> ConsultarAsync(
        ConsultaRequestDto request,
        CancellationToken cancellationToken = default);
}
