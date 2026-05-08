using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class QuartoService : IQuartoService
{
    private readonly IQuartoRepository _quartoRepository;

    public QuartoService(IQuartoRepository quartoRepository)
    {
        _quartoRepository = quartoRepository;
    }

    public Task<IEnumerable<Quarto>> ListarAsync(int? pousadaId = null, CancellationToken cancellationToken = default)
    {
        return _quartoRepository.ListarAsync(pousadaId, cancellationToken);
    }

    public Task<Quarto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _quartoRepository.ObterPorIdComPousadaAsync(id, cancellationToken);
    }

    public async Task<Quarto> CriarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaExistenteAsync(quarto.PousadaId, cancellationToken);
        await _quartoRepository.AdicionarAsync(quarto, cancellationToken);
        return quarto;
    }

    public async Task AtualizarAsync(Quarto quarto, CancellationToken cancellationToken = default)
    {
        await ValidarPousadaExistenteAsync(quarto.PousadaId, cancellationToken);
        await _quartoRepository.AtualizarAsync(quarto, cancellationToken);
    }

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        return _quartoRepository.RemoverPorIdAsync(id, cancellationToken);
    }

    private async Task ValidarPousadaExistenteAsync(int pousadaId, CancellationToken cancellationToken)
    {
        var existe = await _quartoRepository.PousadaExisteAsync(pousadaId, cancellationToken);
        if (!existe)
            throw new InvalidOperationException("Pousada informada não foi encontrada.");
    }
}
