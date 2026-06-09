using System.ClientModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Application.Models;
using PousadaApi.Infrastructure.Options;

namespace PousadaApi.Infrastructure.Integrations;

public sealed class OpenAiLlmClient : ILlmClient
{
    private readonly OpenAiOptions _options;
    private readonly ILogger<OpenAiLlmClient> _logger;

    public OpenAiLlmClient(IOptions<OpenAiOptions> options, ILogger<OpenAiLlmClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<LlmCompletionResult> CompleteAsync(
        IReadOnlyList<LlmMessage> messages,
        IReadOnlyList<LlmToolDefinition> tools,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new ConsultaInteligenteException("Chave da API OpenAI não configurada.");

        var client = new ChatClient(_options.Model, _options.ApiKey);
        var chatMessages = messages.Select(ToChatMessage).ToList();

        var completionOptions = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _options.MaxOutputTokens,
        };

        foreach (var tool in tools)
        {
            completionOptions.Tools.Add(ChatTool.CreateFunctionTool(
                tool.Name,
                tool.Description,
                BinaryData.FromString(tool.ParametersJsonSchema)));
        }

        try
        {
            var completion = await client.CompleteChatAsync(chatMessages, completionOptions, cancellationToken);
            return MapCompletion(completion);
        }
        catch (ClientResultException ex) when (ex.Status == 401)
        {
            throw new ConsultaInteligenteException("Chave da API OpenAI inválida.", ex);
        }
        catch (ClientResultException ex) when (ex.Status == 429)
        {
            throw new ConsultaInteligenteException(
                "Limite de requisições da OpenAI excedido. Tente novamente em instantes.", ex);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
        {
            throw new ConsultaInteligenteException("Tempo esgotado ao consultar a OpenAI.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao chamar a API OpenAI.");
            throw new ConsultaInteligenteException("Serviço de consulta temporariamente indisponível.", ex);
        }
    }

    private static LlmCompletionResult MapCompletion(ChatCompletion completion)
    {
        var content = completion.Content.Count > 0
            ? completion.Content[0].Text
            : null;

        var toolCalls = completion.ToolCalls
            .Select(tc => new LlmToolCall
            {
                Id = tc.Id,
                Name = tc.FunctionName,
                ArgumentsJson = tc.FunctionArguments.ToString(),
            })
            .ToList();

        return new LlmCompletionResult
        {
            Content = content,
            ToolCalls = toolCalls,
        };
    }

    private static ChatMessage ToChatMessage(LlmMessage message)
    {
        return message.Role.ToLowerInvariant() switch
        {
            "system" => new SystemChatMessage(message.Content ?? string.Empty),
            "user" => new UserChatMessage(message.Content ?? string.Empty),
            "assistant" when message.ToolCalls is { Count: > 0 } => CreateAssistantMessage(message),
            "assistant" => new AssistantChatMessage(message.Content ?? string.Empty),
            "tool" => new ToolChatMessage(message.ToolCallId ?? string.Empty, message.Content ?? string.Empty),
            _ => new UserChatMessage(message.Content ?? string.Empty),
        };
    }

    private static AssistantChatMessage CreateAssistantMessage(LlmMessage message)
    {
        var toolCalls = message.ToolCalls!
            .Select(tc => ChatToolCall.CreateFunctionToolCall(
                tc.Id,
                tc.Name,
                BinaryData.FromString(string.IsNullOrWhiteSpace(tc.ArgumentsJson) ? "{}" : tc.ArgumentsJson)))
            .ToList();

        if (!string.IsNullOrWhiteSpace(message.Content))
            return new AssistantChatMessage(toolCalls) { Content = { ChatMessageContentPart.CreateTextPart(message.Content) } };

        return new AssistantChatMessage(toolCalls);
    }
}
