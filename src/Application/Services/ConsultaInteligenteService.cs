using PousadaApi.Application.DTOs;
using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Application.Models;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public sealed class ConsultaInteligenteService : IConsultaInteligenteService
{
    private const int MaxToolRounds = 3;

    private readonly IPousadaRepository _pousadaRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILlmClient _llmClient;
    private readonly ConsultaToolExecutor _toolExecutor;

    public ConsultaInteligenteService(
        IPousadaRepository pousadaRepository,
        ICurrentUserService currentUser,
        ILlmClient llmClient,
        ConsultaToolExecutor toolExecutor)
    {
        _pousadaRepository = pousadaRepository;
        _currentUser = currentUser;
        _llmClient = llmClient;
        _toolExecutor = toolExecutor;
    }

    public async Task<ConsultaResponseDto> ConsultarAsync(
        ConsultaRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var pertence = await _pousadaRepository.PertenceAoUsuarioAsync(
            request.PousadaId, _currentUser.UserId, cancellationToken);

        if (!pertence)
            throw new AcessoNegadoException();

        var messages = MontarMensagens(request);
        var tools = ConsultaToolDefinitions.ObterTodas();
        var ferramentasUsadas = new List<string>();
        ConsultaPeriodoDto? periodoConsultado = null;

        for (var round = 0; round < MaxToolRounds; round++)
        {
            var completion = await _llmClient.CompleteAsync(messages, tools, cancellationToken);

            if (completion.ToolCalls.Count == 0)
            {
                return new ConsultaResponseDto
                {
                    Resposta = string.IsNullOrWhiteSpace(completion.Content)
                        ? "Não foi possível gerar uma resposta para a sua pergunta."
                        : completion.Content,
                    FerramentasUsadas = ferramentasUsadas.Distinct().ToList(),
                    PeriodoConsultado = periodoConsultado,
                };
            }

            messages.Add(new LlmMessage
            {
                Role = "assistant",
                Content = completion.Content,
                ToolCalls = completion.ToolCalls,
            });

            foreach (var toolCall in completion.ToolCalls)
            {
                ferramentasUsadas.Add(toolCall.Name);

                var execution = await _toolExecutor.ExecutarAsync(
                    toolCall.Name,
                    request.PousadaId,
                    toolCall.ArgumentsJson,
                    cancellationToken);

                if (execution.PeriodoConsultado is not null)
                    periodoConsultado = execution.PeriodoConsultado;

                messages.Add(new LlmMessage
                {
                    Role = "tool",
                    ToolCallId = toolCall.Id,
                    Content = execution.ResultJson,
                });
            }
        }

        return new ConsultaResponseDto
        {
            Resposta = "Não foi possível concluir a consulta no número máximo de etapas. Tente reformular a pergunta com um período mais específico.",
            FerramentasUsadas = ferramentasUsadas.Distinct().ToList(),
            PeriodoConsultado = periodoConsultado,
        };
    }

    private static List<LlmMessage> MontarMensagens(ConsultaRequestDto request)
    {
        var messages = new List<LlmMessage>
        {
            new() { Role = "system", Content = MontarSystemPrompt() },
        };

        if (request.Historico is not null)
        {
            foreach (var item in request.Historico)
            {
                messages.Add(new LlmMessage
                {
                    Role = item.Role,
                    Content = item.Conteudo,
                });
            }
        }

        messages.Add(new LlmMessage
        {
            Role = "user",
            Content = request.Pergunta,
        });

        return messages;
    }

    private static string MontarSystemPrompt()
    {
        var agoraUtc = DateTime.UtcNow;
        return $"""
            Você é um assistente para gestores de pousadas de pequeno e médio porte.
            Data e hora atuais (UTC): {agoraUtc:yyyy-MM-dd HH:mm:ss}.

            Regras:
            - Responda sempre em português do Brasil, de forma clara e objetiva.
            - Use apenas os dados retornados pelas ferramentas disponíveis. Não invente números, datas ou nomes.
            - Formate valores monetários em reais (R$).
            - Formate datas de forma legível para o gestor.
            - Interprete expressões relativas de tempo (ex.: "este mês", "março de 2026", "última semana") com base na data atual informada acima.
            - Se a pergunta não puder ser respondida com as ferramentas disponíveis, explique educadamente o que você consegue consultar.
            - Não mencione detalhes técnicos internos (API, banco de dados, ferramentas, SQL).
            """;
    }
}
