using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;
using PousadaApi.Application.Options;

namespace PousadaApi.Application.Services;

public sealed class RecuperacaoSenhaService : IRecuperacaoSenhaService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioRecuperacaoSenhaRepository _recuperacaoRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHasher _tokenHasher;
    private readonly IEmailService _emailService;
    private readonly AppOptions _appOptions;
    private readonly ILogger<RecuperacaoSenhaService> _logger;

    public RecuperacaoSenhaService(
        IUsuarioRepository usuarioRepository,
        IUsuarioRecuperacaoSenhaRepository recuperacaoRepository,
        IPasswordHasher passwordHasher,
        ITokenHasher tokenHasher,
        IEmailService emailService,
        IOptions<AppOptions> appOptions,
        ILogger<RecuperacaoSenhaService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _recuperacaoRepository = recuperacaoRepository;
        _passwordHasher = passwordHasher;
        _tokenHasher = tokenHasher;
        _emailService = emailService;
        _appOptions = appOptions.Value;
        _logger = logger;
    }

    public Task SolicitarPorUsuarioAsync(int userId, CancellationToken cancellationToken = default)
    {
        return SolicitarInternoAsync(userId, cancellationToken);
    }

    public async Task SolicitarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        var usuario = await _usuarioRepository.ObterPorEmailAsync(email.Trim(), cancellationToken);
        if (usuario is null)
            return;

        await EnviarTokenAsync(usuario.Id, usuario.Email, cancellationToken);
    }

    public async Task RedefinirAsync(string token, string novaSenha, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(novaSenha))
            throw new InvalidOperationException("Token e nova senha são obrigatórios.");

        var tokenHash = _tokenHasher.Hash(token);
        var recuperacao = await _recuperacaoRepository.ObterValidoPorTokenHashAsync(tokenHash, cancellationToken);
        if (recuperacao is null)
            throw new InvalidOperationException("Token inválido ou expirado.");

        recuperacao.Usuario.SenhaHash = _passwordHasher.Hash(novaSenha);
        await _usuarioRepository.AtualizarAsync(recuperacao.Usuario, cancellationToken);
        await _recuperacaoRepository.MarcarComoUsadoAsync(recuperacao, cancellationToken);
    }

    private async Task SolicitarInternoAsync(int userId, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(userId, cancellationToken);
        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        await EnviarTokenAsync(usuario.Id, usuario.Email, cancellationToken);
    }

    private async Task EnviarTokenAsync(int usuarioId, string email, CancellationToken cancellationToken)
    {
        var token = GerarToken();
        var tokenHash = _tokenHasher.Hash(token);

        var recuperacao = new UsuarioRecuperacaoSenha
        {
            UsuarioId = usuarioId,
            TokenHash = tokenHash,
            ExpiraEm = DateTime.UtcNow.AddHours(_appOptions.RecuperacaoSenhaExpiracaoHoras)
        };

        await _recuperacaoRepository.AdicionarAsync(recuperacao, cancellationToken);

        var baseUrl = _appOptions.FrontendBaseUrl.TrimEnd('/');
        var link = $"{baseUrl}/redefinir-senha?token={Uri.EscapeDataString(token)}";

        try
        {
            await _emailService.EnviarRedefinicaoSenhaAsync(
                email,
                link,
                _appOptions.RecuperacaoSenhaExpiracaoHoras,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de redefinição para {Email}", email);
            throw new InvalidOperationException("Não foi possível enviar o e-mail de redefinição. Verifique a configuração de e-mail (Resend).");
        }
    }

    private static string GerarToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
