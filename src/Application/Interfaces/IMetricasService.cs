using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Interfaces;

public interface IMetricasService
{
    Task<MetricasReadDto> ObterAsync(int pousadaId, DateTime de, DateTime ate, CancellationToken cancellationToken = default);
}
