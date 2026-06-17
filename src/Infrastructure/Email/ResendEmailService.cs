using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PousadaApi.Application.Interfaces;
using PousadaApi.Infrastructure.Options;
using Resend;

namespace PousadaApi.Infrastructure.Email;

public sealed class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IOptions<ResendOptions> options, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarRedefinicaoSenhaAsync(
        string destinatario,
        string linkRedefinicao,
        int validadeHoras,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.From))
            throw new InvalidOperationException("Resend não configurado. Defina Resend:From nas configurações.");

        var prazoValidade = validadeHoras == 1 ? "1 hora" : $"{validadeHoras} horas";

        var corpo = $"""
            Olá,

            Recebemos uma solicitação para redefinir sua senha.

            Clique no link abaixo para criar uma nova senha (válido por {prazoValidade}):
            {linkRedefinicao}

            Se você não solicitou esta alteração, ignore este e-mail.

            Equipe Pousada
            """;

        var message = new EmailMessage
        {
            From = _options.From,
            Subject = "Redefinição de senha — Pousada",
            TextBody = corpo
        };
        message.To.Add(destinatario);

        var emailId = await _resend.EmailSendAsync(message, cancellationToken);

        _logger.LogInformation(
            "E-mail de redefinição enviado para {Destinatario}, id={EmailId}",
            destinatario,
            emailId);
    }
}
