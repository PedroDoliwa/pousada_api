namespace PousadaApi.Application.Models;

public sealed class LlmToolCall
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ArgumentsJson { get; set; } = "{}";
}
