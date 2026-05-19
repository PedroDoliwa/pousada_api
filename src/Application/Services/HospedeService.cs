using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class HospedeService : IHospedeService
{
    private readonly IHospedeRepository _hospedeRepository;
    private readonly ICurrentUserService _currentUser;

    public HospedeService(IHospedeRepository hospedeRepository, ICurrentUserService currentUser)
    {
        _hospedeRepository = hospedeRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<Hospede>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        if (pousadaId.HasValue)
            await ValidarPousadaDoUsuarioAsync(pousadaId.Value, cancellationToken);

        return await _hospedeRepository.ListarPorUsuarioAsync(_currentUser.UserId, pousadaId, cancellationToken);
    }

    public Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _hospedeRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
    }

    public async Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaDoUsuarioAsync(hospede.PousadaId, cancellationToken);
        await _hospedeRepository.AdicionarAsync(hospede, cancellationToken);
        return hospede;
    }

    public async Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        var existente = await _hospedeRepository.ObterPorIdEUsuarioAsync(hospede.Id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        await ValidarPousadaDoUsuarioAsync(hospede.PousadaId, cancellationToken);
        await _hospedeRepository.AtualizarAsync(hospede, cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _hospedeRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        await _hospedeRepository.RemoverPorIdAsync(id, cancellationToken);
    }

    private async Task ValidarPousadaDoUsuarioAsync(int pousadaId, CancellationToken cancellationToken)
    {
        var pertence = await _hospedeRepository.PousadaPertenceAoUsuarioAsync(pousadaId, _currentUser.UserId, cancellationToken);
        if (!pertence)
            throw new AcessoNegadoException();
    }
}
