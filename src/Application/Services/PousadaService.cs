using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class PousadaService : IPousadaService
{
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
}
