using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PousadaApi.Application.Interfaces;
using PousadaApi.Infrastructure.Options;

namespace PousadaApi.Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnviarRedefinicaoSenhaAsync(string destinatario, string linkRedefinicao, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
            throw new InvalidOperationException("SMTP não configurado. Defina Smtp:Host nas configurações.");

        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(_options.From));
        mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = "Redefinição de senha — Pousada";

        var corpo = $"""
            Olá,

            Recebemos uma solicitação para redefinir sua senha.

            Clique no link abaixo para criar uma nova senha (válido por 1 hora):
            {linkRedefinicao}

            Se você não solicitou esta alteração, ignore este e-mail.

            Equipe Pousada
            """;

        mensagem.Body = new TextPart("plain") { Text = corpo };

        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(_options.Host, _options.Port, _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.User))
            await cliente.AuthenticateAsync(_options.User, _options.Password, cancellationToken);

        await cliente.SendAsync(mensagem, cancellationToken);
        await cliente.DisconnectAsync(true, cancellationToken);

        _logger.LogInformation("E-mail de redefinição enviado para {Destinatario}", destinatario);
    }
}
