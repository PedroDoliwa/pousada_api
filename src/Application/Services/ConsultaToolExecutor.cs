using System.Globalization;
using System.Text.Json;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Application.Models;
using PousadaApi.Domain.Constants;

namespace PousadaApi.Application.Services;

public sealed class ConsultaToolExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IMetricasService _metricasService;
    private readonly IDisponibilidadeService _disponibilidadeService;
    private readonly IQuartoService _quartoService;
    private readonly IHospedeService _hospedeService;

    public ConsultaToolExecutor(
        IMetricasService metricasService,
        IDisponibilidadeService disponibilidadeService,
        IQuartoService quartoService,
        IHospedeService hospedeService)
    {
        _metricasService = metricasService;
        _disponibilidadeService = disponibilidadeService;
        _quartoService = quartoService;
        _hospedeService = hospedeService;
    }

    public async Task<ConsultaToolExecutionResult> ExecutarAsync(
        string toolName,
        int pousadaId,
        string argumentsJson,
        CancellationToken cancellationToken = default)
    {
        using var document = ParseArguments(argumentsJson);
        var args = document.RootElement;

        return toolName switch
        {
            ConsultaToolDefinitions.ObterMetricas => await ObterMetricasAsync(pousadaId, args, cancellationToken),
            ConsultaToolDefinitions.ListarOcupacao => await ListarOcupacaoAsync(pousadaId, args, cancellationToken),
            ConsultaToolDefinitions.ContarReservasPorOrigem => await ContarReservasPorOrigemAsync(pousadaId, args, cancellationToken),
            ConsultaToolDefinitions.ListarQuartos => await ListarQuartosAsync(pousadaId, cancellationToken),
            ConsultaToolDefinitions.ListarHospedes => await ListarHospedesAsync(pousadaId, cancellationToken),
            _ => throw new InvalidOperationException($"Ferramenta desconhecida: {toolName}."),
        };
    }

    private async Task<ConsultaToolExecutionResult> ObterMetricasAsync(
        int pousadaId,
        JsonElement args,
        CancellationToken cancellationToken)
    {
        var (de, ate) = ParsePeriodo(args);
        var metricas = await _metricasService.ObterAsync(pousadaId, de, ate, cancellationToken);

        return new ConsultaToolExecutionResult
        {
            ResultJson = JsonSerializer.Serialize(metricas, JsonOptions),
            PeriodoConsultado = new ConsultaPeriodoDto { De = de, Ate = ate },
        };
    }

    private async Task<ConsultaToolExecutionResult> ListarOcupacaoAsync(
        int pousadaId,
        JsonElement args,
        CancellationToken cancellationToken)
    {
        var (de, ate) = ParsePeriodo(args);
        var ocupacao = (await _disponibilidadeService.ListarOcupacaoAsync(pousadaId, de, ate, cancellationToken)).ToList();

        var payload = new
        {
            total = ocupacao.Count,
            reservas = ocupacao.Select(o => new
            {
                o.QuartoNumeroOuNome,
                o.HospedeNome,
                o.DataEntrada,
                o.DataSaida,
                o.Status,
                o.Origem,
                o.TituloExterno,
            }),
        };

        return new ConsultaToolExecutionResult
        {
            ResultJson = JsonSerializer.Serialize(payload, JsonOptions),
            PeriodoConsultado = new ConsultaPeriodoDto { De = de, Ate = ate },
        };
    }

    private async Task<ConsultaToolExecutionResult> ContarReservasPorOrigemAsync(
        int pousadaId,
        JsonElement args,
        CancellationToken cancellationToken)
    {
        var (de, ate) = ParsePeriodo(args);
        var ocupacao = await _disponibilidadeService.ListarOcupacaoAsync(pousadaId, de, ate, cancellationToken);

        var contagens = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            [ReservaOrigens.Manual] = 0,
            [ReservaOrigens.Airbnb] = 0,
            [ReservaOrigens.Booking] = 0,
            [ReservaOrigens.Outro] = 0,
        };

        foreach (var reserva in ocupacao)
        {
            var origem = NormalizarOrigem(reserva.Origem);
            contagens[origem]++;
        }

        var payload = new
        {
            total = contagens.Values.Sum(),
            manual = contagens[ReservaOrigens.Manual],
            airbnb = contagens[ReservaOrigens.Airbnb],
            booking = contagens[ReservaOrigens.Booking],
            outro = contagens[ReservaOrigens.Outro],
        };

        return new ConsultaToolExecutionResult
        {
            ResultJson = JsonSerializer.Serialize(payload, JsonOptions),
            PeriodoConsultado = new ConsultaPeriodoDto { De = de, Ate = ate },
        };
    }

    private async Task<ConsultaToolExecutionResult> ListarQuartosAsync(
        int pousadaId,
        CancellationToken cancellationToken)
    {
        var quartos = (await _quartoService.ListarAsync(pousadaId, cancellationToken)).ToList();

        var payload = new
        {
            total = quartos.Count,
            quartos = quartos.Select(q => new
            {
                q.NumeroOuNome,
                q.Capacidade,
                q.ValorDiaria,
                q.Status,
            }),
        };

        return new ConsultaToolExecutionResult
        {
            ResultJson = JsonSerializer.Serialize(payload, JsonOptions),
        };
    }

    private async Task<ConsultaToolExecutionResult> ListarHospedesAsync(
        int pousadaId,
        CancellationToken cancellationToken)
    {
        var hospedes = (await _hospedeService.ListarAsync(pousadaId, cancellationToken)).ToList();

        var payload = new
        {
            total = hospedes.Count,
            hospedes = hospedes.Select(h => new { h.Nome }),
        };

        return new ConsultaToolExecutionResult
        {
            ResultJson = JsonSerializer.Serialize(payload, JsonOptions),
        };
    }

    private static string NormalizarOrigem(string origem) =>
        origem.ToLowerInvariant() switch
        {
            var o when o == ReservaOrigens.Airbnb.ToLowerInvariant() => ReservaOrigens.Airbnb,
            var o when o == ReservaOrigens.Booking.ToLowerInvariant() => ReservaOrigens.Booking,
            var o when o == ReservaOrigens.Manual.ToLowerInvariant() => ReservaOrigens.Manual,
            _ => ReservaOrigens.Outro,
        };

    private static (DateTime De, DateTime Ate) ParsePeriodo(JsonElement args)
    {
        var de = ParseDateArgument(args, "de");
        var ate = ParseDateArgument(args, "ate");

        if (de >= ate)
            throw new InvalidOperationException("A data final deve ser posterior à data inicial.");

        return (de, ate);
    }

    private static DateTime ParseDateArgument(JsonElement args, string propertyName)
    {
        if (!args.TryGetProperty(propertyName, out var property))
            throw new InvalidOperationException($"Parâmetro '{propertyName}' é obrigatório.");

        var raw = property.GetString();
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException($"Parâmetro '{propertyName}' é obrigatório.");

        if (DateOnly.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            return DateTime.SpecifyKind(dateOnly.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        return DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
    }

    private static JsonDocument ParseArguments(string argumentsJson)
    {
        if (string.IsNullOrWhiteSpace(argumentsJson))
            argumentsJson = "{}";

        try
        {
            return JsonDocument.Parse(argumentsJson);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Argumentos da ferramenta inválidos.", ex);
        }
    }
}
