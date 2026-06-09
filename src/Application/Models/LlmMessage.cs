namespace PousadaApi.Application.Models;

public sealed class LlmMessage
{
    public string Role { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? ToolCallId { get; set; }

    public IReadOnlyList<LlmToolCall>? ToolCalls { get; set; }
}
