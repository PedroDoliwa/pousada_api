namespace PousadaApi.Application.Models;

public sealed class LlmToolDefinition
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ParametersJsonSchema { get; set; } = "{}";
}
