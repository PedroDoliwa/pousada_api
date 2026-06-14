using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public sealed class UsuarioService : IUsuarioService
{
    private const int TamanhoMaximoFotoBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> TiposFotoPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioReadDto> ObterPerfilAsync(int userId, CancellationToken cancellationToken = default)
    {
        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);
        return MapearPerfil(usuario);
    }

    public async Task<Usuario> AtualizarNomeAsync(int userId, string nome, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("Nome é obrigatório.");

        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);
        usuario.Nome = nome.Trim();
        await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);
        return usuario;
    }

    public async Task AlterarSenhaAsync(int userId, string senhaAtual, string senhaNova, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(senhaAtual) || string.IsNullOrWhiteSpace(senhaNova))
            throw new InvalidOperationException("Senha atual e nova senha são obrigatórias.");

        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);

        if (!_passwordHasher.Verify(senhaAtual, usuario.SenhaHash))
            throw new InvalidOperationException("Senha atual incorreta.");

        usuario.SenhaHash = _passwordHasher.Hash(senhaNova);
        await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);
    }

    public async Task SalvarFotoAsync(int userId, byte[] bytes, string contentType, CancellationToken cancellationToken = default)
    {
        ValidarFoto(bytes, contentType);

        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);
        usuario.Foto = bytes;
        usuario.FotoContentType = contentType;
        await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);
    }

    public async Task<(byte[]? Bytes, string? ContentType)> ObterFotoAsync(int userId, CancellationToken cancellationToken = default)
    {
        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);
        if (usuario.Foto is null || usuario.Foto.Length == 0)
            return (null, null);

        return (usuario.Foto, usuario.FotoContentType);
    }

    public async Task RemoverFotoAsync(int userId, CancellationToken cancellationToken = default)
    {
        var usuario = await ObterUsuarioOuFalharAsync(userId, cancellationToken);
        usuario.Foto = null;
        usuario.FotoContentType = null;
        await _usuarioRepository.AtualizarAsync(usuario, cancellationToken);
    }

    private async Task<Usuario> ObterUsuarioOuFalharAsync(int userId, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(userId, cancellationToken);
        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        return usuario;
    }

    private static void ValidarFoto(byte[] bytes, string contentType)
    {
        if (bytes.Length == 0)
            throw new InvalidOperationException("Arquivo de foto vazio.");

        if (bytes.Length > TamanhoMaximoFotoBytes)
            throw new InvalidOperationException("A foto deve ter no máximo 2 MB.");

        if (!TiposFotoPermitidos.Contains(contentType))
            throw new InvalidOperationException("Formato de foto não suportado. Use JPEG, PNG ou WebP.");
    }

    private static UsuarioReadDto MapearPerfil(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = usuario.Perfil,
        TemFoto = usuario.Foto is { Length: > 0 }
    };
}
