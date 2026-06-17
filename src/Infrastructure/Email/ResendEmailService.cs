using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PousadaApi.Application.Interfaces;
using PousadaApi.Infrastructure.Options;
using Resend;

namespace PousadaApi.Infrastructure.Email;

public sealed class ResendEmailService : IEmailService
{
    private const int ValidadeHoras = 1;

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

        var message = new EmailMessage
        {
            From = _options.From,
            Subject = "Redefinição de senha — Hospedinn",
            // Texto puro como fallback (clientes que bloqueiam HTML e melhor entregabilidade).
            TextBody = MontarTextoSimples(linkRedefinicao),
            HtmlBody = MontarHtml(linkRedefinicao)
        };
        message.To.Add(destinatario);

        var emailId = await _resend.EmailSendAsync(message, cancellationToken);

        _logger.LogInformation(
            "E-mail de redefinição enviado para {Destinatario}, id={EmailId}",
            destinatario,
            emailId);
    }

    private static string MontarTextoSimples(string link) => $"""
        Redefinição de senha — Hospedinn

        Olá,

        Recebemos uma solicitação para redefinir a senha da sua conta.

        Para criar uma nova senha, acesse o link abaixo (válido por {ValidadeHoras} hora):
        {link}

        Se você não solicitou esta alteração, ignore este e-mail com segurança —
        sua senha atual continua válida.

        Equipe Hospedinn
        """;

    // HTML com estilos inline e layout em tabela para compatibilidade com os principais
    // clientes de e-mail (Gmail, Outlook, Apple Mail), que ignoram <style>/CSS externo.
    private static string MontarHtml(string link)
    {
        var href = WebUtility.HtmlEncode(link);

        return $"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1.0">
              <title>Redefinição de senha</title>
            </head>
            <body style="margin:0; padding:0; background-color:#f4f5f7; font-family:Arial, Helvetica, sans-serif;">
              <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f5f7;">
                <tr>
                  <td align="center" style="padding:32px 16px;">
                    <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:480px; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 1px 4px rgba(0,0,0,0.08);">
                      <tr>
                        <td style="background-color:#1d4ed8; padding:24px 32px; text-align:center;">
                          <span style="color:#ffffff; font-size:20px; font-weight:bold; letter-spacing:0.5px;">Hospedinn</span>
                        </td>
                      </tr>
                      <tr>
                        <td style="padding:32px;">
                          <h1 style="margin:0 0 16px; font-size:20px; color:#1a1a1a;">Redefinição de senha</h1>
                          <p style="margin:0 0 16px; font-size:15px; line-height:1.6; color:#444444;">
                            Olá,
                          </p>
                          <p style="margin:0 0 24px; font-size:15px; line-height:1.6; color:#444444;">
                            Recebemos uma solicitação para redefinir a senha da sua conta.
                            Clique no botão abaixo para criar uma nova senha. Este link é válido por
                            <strong>{ValidadeHoras} hora</strong>.
                          </p>
                          <table role="presentation" cellpadding="0" cellspacing="0" style="margin:0 auto 24px;">
                            <tr>
                              <td align="center" style="border-radius:8px; background-color:#1d4ed8;">
                                <a href="{href}" target="_blank"
                                   style="display:inline-block; padding:14px 32px; font-size:16px; font-weight:bold; color:#ffffff; text-decoration:none; border-radius:8px;">
                                  Redefinir minha senha
                                </a>
                              </td>
                            </tr>
                          </table>
                          <hr style="border:none; border-top:1px solid #eeeeee; margin:0 0 16px;">
                          <p style="margin:0; font-size:13px; line-height:1.6; color:#999999;">
                            🔒 Se você não solicitou esta alteração, ignore este e-mail com segurança —
                            sua senha atual continua válida e nenhuma ação é necessária.
                          </p>
                        </td>
                      </tr>
                      <tr>
                        <td style="background-color:#fafafa; padding:16px 32px; text-align:center;">
                          <p style="margin:0; font-size:12px; color:#aaaaaa;">
                            Equipe Hospedinn · Este é um e-mail automático, não responda.
                          </p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;
    }
}
