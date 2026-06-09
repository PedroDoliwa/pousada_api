using PousadaApi.Application.Models;

namespace PousadaApi.Application.Services;

public static class ConsultaToolDefinitions
{
    public const string ObterMetricas = "obter_metricas";
    public const string ListarOcupacao = "listar_ocupacao";
    public const string ContarReservasPorOrigem = "contar_reservas_por_origem";
    public const string ListarQuartos = "listar_quartos";
    public const string ListarHospedes = "listar_hospedes";

    private const string PeriodoParametersSchema = """
        {
            "type": "object",
            "properties": {
                "de": {
                    "type": "string",
                    "format": "date",
                    "description": "Data inicial do período (inclusiva), formato ISO 8601, ex.: 2026-03-01"
                },
                "ate": {
                    "type": "string",
                    "format": "date",
                    "description": "Data final do período (exclusiva), formato ISO 8601, ex.: 2026-04-01"
                }
            },
            "required": ["de", "ate"]
        }
        """;

    private const string EmptyParametersSchema = """
        {
            "type": "object",
            "properties": {}
        }
        """;

    public static IReadOnlyList<LlmToolDefinition> ObterTodas() =>
    [
        new LlmToolDefinition
        {
            Name = ObterMetricas,
            Description = "Obtém métricas da pousada em um período: faturamento total, taxa de ocupação (%), total de reservas confirmadas e hóspedes únicos.",
            ParametersJsonSchema = PeriodoParametersSchema,
        },
        new LlmToolDefinition
        {
            Name = ListarOcupacao,
            Description = "Lista reservas ativas que interceptam o período informado, com quarto, hóspede, datas, status e origem (Manual, Airbnb, Booking, Outro).",
            ParametersJsonSchema = PeriodoParametersSchema,
        },
        new LlmToolDefinition
        {
            Name = ContarReservasPorOrigem,
            Description = "Conta reservas ativas no período agrupadas por origem/canal: manual, Airbnb, Booking e outros.",
            ParametersJsonSchema = PeriodoParametersSchema,
        },
        new LlmToolDefinition
        {
            Name = ListarQuartos,
            Description = "Lista os quartos cadastrados na pousada com número/nome, capacidade, valor da diária e status.",
            ParametersJsonSchema = EmptyParametersSchema,
        },
        new LlmToolDefinition
        {
            Name = ListarHospedes,
            Description = "Lista os hóspedes cadastrados na pousada (nome e total).",
            ParametersJsonSchema = EmptyParametersSchema,
        },
    ];
}
