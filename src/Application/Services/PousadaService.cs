using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class PousadaService : IPousadaService
{
    private const int TamanhoMaximoFotoBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> TiposFotoPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly IPousadaRepository _pousadaRepository;
    private readonly ICurrentUserService _currentUser;

    public PousadaService(IPousadaRepository pousadaRepository, ICurrentUserService currentUser)
    {
        _pousadaRepository = pousadaRepository;
        _currentUser = currentUser;
    }

    public Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _pousadaRepository.ListarPorUsuarioAsync(_currentUser.UserId, cancellationToken);
    }

    public Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _pousadaRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
    }

    public async Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        pousada.UsuarioId = _currentUser.UserId;
        await _pousadaRepository.AdicionarAsync(pousada, cancellationToken);
        return pousada;
    }

    public async Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        var existente = await _pousadaRepository.ObterPorIdEUsuarioAsync(pousada.Id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        pousada.UsuarioId = _currentUser.UserId;
        await _pousadaRepository.AtualizarAsync(pousada, cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _pousadaRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        await _pousadaRepository.RemoverPorIdAsync(id, cancellationToken);
    }

    public async Task SalvarFotoAsync(int id, byte[] bytes, string contentType, CancellationToken cancellationToken = default)
    {
        ValidarFoto(bytes, contentType);

        var pousada = await ObterPousadaDoUsuarioOuFalharAsync(id, cancellationToken);
        pousada.Foto = bytes;
        pousada.FotoContentType = contentType;
        await _pousadaRepository.AtualizarAsync(pousada, cancellationToken);
    }

    public async Task<(byte[]? Bytes, string? ContentType)> ObterFotoAsync(int id, CancellationToken cancellationToken = default)
    {
        var pousada = await ObterPousadaDoUsuarioOuFalharAsync(id, cancellationToken);
        if (pousada.Foto is null || pousada.Foto.Length == 0)
            return (null, null);

        return (pousada.Foto, pousada.FotoContentType);
    }

    public async Task RemoverFotoAsync(int id, CancellationToken cancellationToken = default)
    {
        var pousada = await ObterPousadaDoUsuarioOuFalharAsync(id, cancellationToken);
        pousada.Foto = null;
        pousada.FotoContentType = null;
        await _pousadaRepository.AtualizarAsync(pousada, cancellationToken);
    }

    private async Task<Pousada> ObterPousadaDoUsuarioOuFalharAsync(int id, CancellationToken cancellationToken)
    {
        var pousada = await _pousadaRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (pousada is null)
            throw new AcessoNegadoException();

        return pousada;
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
}
