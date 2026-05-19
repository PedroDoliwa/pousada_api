using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class QuartoService : IQuartoService
{
    private readonly IQuartoRepository _quartoRepository;
    private readonly IPousadaRepository _pousadaRepository;
    private readonly ICurrentUserService _currentUser;

    public QuartoService(
        IQuartoRepository quartoRepository,
        IPousadaRepository pousadaRepository,
        ICurrentUserService currentUser)
    {
        _quartoRepository = quartoRepository;
        _pousadaRepository = pousadaRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        if (pousadaId.HasValue)
            await ValidarPousadaDoUsuarioAsync(pousadaId.Value, cancellationToken);

        return await _quartoRepository.ListarPorUsuarioAsync(_currentUser.UserId, pousadaId, cancellationToken);
    }

    public Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _quartoRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
    }

    public async Task<Quarto> CriarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaDoUsuarioAsync(quarto.PousadaId, cancellationToken);
        await _quartoRepository.AdicionarAsync(quarto, cancellationToken);
        return quarto;
    }

    public async Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        var existente = await _quartoRepository.ObterPorIdEUsuarioAsync(quarto.Id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        await ValidarPousadaDoUsuarioAsync(quarto.PousadaId, cancellationToken);
        await _quartoRepository.AtualizarAsync(quarto, cancellationToken);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _quartoRepository.ObterPorIdEUsuarioAsync(id, _currentUser.UserId, cancellationToken);
        if (existente is null)
            throw new AcessoNegadoException();

        await _quartoRepository.RemoverPorIdAsync(id, cancellationToken);
    }

    private async Task ValidarPousadaDoUsuarioAsync(int pousadaId, CancellationToken cancellationToken)
    {
        var pertence = await _pousadaRepository.PertenceAoUsuarioAsync(pousadaId, _currentUser.UserId, cancellationToken);
        if (!pertence)
            throw new AcessoNegadoException();
    }
}
