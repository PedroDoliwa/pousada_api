namespace PousadaApi.Application.Models;

public sealed class LlmCompletionResult
{
    public string? Content { get; set; }

    public IReadOnlyList<LlmToolCall> ToolCalls { get; set; } = [];
}
