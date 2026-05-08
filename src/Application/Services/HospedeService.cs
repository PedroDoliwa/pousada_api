using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class HospedeService : IHospedeService
{
    private readonly IHospedeRepository _hospedeRepository;

    public HospedeService(IHospedeRepository hospedeRepository)
    {
        _hospedeRepository = hospedeRepository;
    }

    public Task<IEnumerable<Hospede>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _hospedeRepository.ListarAsync(cancellationToken);
    }

    public Task<Hospede?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _hospedeRepository.ObterPorIdAsync(id, cancellationToken);
    }

    public async Task<Hospede> CriarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        await _hospedeRepository.AdicionarAsync(hospede, cancellationToken);
        return hospede;
    }

    public Task AtualizarAsync(Hospede hospede, CancellationToken cancellationToken = default)
    {
        return _hospedeRepository.AtualizarAsync(hospede, cancellationToken);
    }

    public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        return _hospedeRepository.RemoverPorIdAsync(id, cancellationToken);
    }
}
