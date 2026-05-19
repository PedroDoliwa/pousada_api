namespace PousadaApi.Application.Interfaces;

public interface IIcalExportService
{
    Task<string> GerarCalendarioQuartoAsync(int quartoId, CancellationToken cancellationToken = default);
    Task<string> GerarCalendarioPorTokenAsync(string tokenExportacao, CancellationToken cancellationToken = default);
}
