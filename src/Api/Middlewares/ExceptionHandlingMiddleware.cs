using System.Net.Mime;
using System.Text.Json;
using PousadaApi.Application.Exceptions;

namespace PousadaApi.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, message) = exception switch
        {
            AcessoNegadoException => (StatusCodes.Status404NotFound, "Recurso não encontrado."),
            ConsultaInteligenteException consulta => (StatusCodes.Status503ServiceUnavailable, consulta.Message),
            InvalidOperationException inv => (StatusCodes.Status400BadRequest, inv.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Usuário não autenticado."),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar a solicitação.")
        };

        if (context.Response.HasStarted)
            return Task.CompletedTask;

        context.Response.StatusCode = status;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        return context.Response.WriteAsJsonAsync(new { message }, JsonOptions);
    }
}
