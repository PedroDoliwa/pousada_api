using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class PousadaService : IPousadaService
{
    private readonly IPousadaRepository _pousadaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PousadaService(IPousadaRepository pousadaRepository, IUsuarioRepository usuarioRepository)
    {
        _pousadaRepository = pousadaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public Task<IEnumerable<Pousada>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _pousadaRepository.ListarComQuartosAsync(cancellationToken);
    }

    public Task<Pousada?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _pousadaRepository.ObterPorIdComQuartosAsync(id, cancellationToken);
    }

    public async Task<Pousada> CriarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        await ValidarUsuarioExistenteAsync(pousada.UsuarioId, cancellationToken);
        await _pousadaRepository.AdicionarAsync(pousada, cancellationToken);
        return pousada;
    }

    public async Task AtualizarAsync(Pousada pousada, CancellationToken cancellationToken = default)
    {
        await ValidarUsuarioExistenteAsync(pousada.UsuarioId, cancellationToken);
        await _pousadaRepository.AtualizarAsync(pousada, cancellationToken);
    }

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        return _pousadaRepository.RemoverPorIdAsync(id, cancellationToken);
    }

    private async Task ValidarUsuarioExistenteAsync(int usuarioId, CancellationToken cancellationToken)
    {
        var existe = await _usuarioRepository.ExistePorIdAsync(usuarioId, cancellationToken);
        if (!existe)
            throw new InvalidOperationException("Usuário informado não foi encontrado.");
    }
}
