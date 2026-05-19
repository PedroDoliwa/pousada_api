using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.BackgroundServices;

public sealed class CalendarioSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CalendarioSyncBackgroundService> _logger;

    public CalendarioSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<CalendarioSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var habilitado = _configuration.GetValue("CalendarioSync:Habilitado", true);
        if (!habilitado)
            return;

        var intervaloMinutos = _configuration.GetValue("CalendarioSync:IntervaloMinutos", 30);
        var intervalo = TimeSpan.FromMinutes(Math.Max(1, intervaloMinutos));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var calendarioService = scope.ServiceProvider.GetRequiredService<ICalendarioExternoService>();
                await calendarioService.SincronizarTodosAtivosAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha na sincronização periódica de calendários iCal");
            }

            await Task.Delay(intervalo, stoppingToken);
        }
    }
}
