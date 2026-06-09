using PousadaApi.Application.Models;

namespace PousadaApi.Application.Interfaces;

public interface ILlmClient
{
    Task<LlmCompletionResult> CompleteAsync(
        IReadOnlyList<LlmMessage> messages,
        IReadOnlyList<LlmToolDefinition> tools,
        CancellationToken cancellationToken = default);
}
