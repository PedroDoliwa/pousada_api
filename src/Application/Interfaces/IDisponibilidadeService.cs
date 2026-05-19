using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Interfaces;

public interface IDisponibilidadeService
{
    Task<bool> QuartoDisponivelAsync(
        int quartoId,
        DateTime dataEntrada,
        DateTime dataSaida,
        int? reservaIdIgnorar = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<OcupacaoReadDto>> ListarOcupacaoAsync(
        int pousadaId,
        DateTime de,
        DateTime ate,
        CancellationToken cancellationToken = default);
}
