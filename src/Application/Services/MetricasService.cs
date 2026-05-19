using PousadaApi.Application.DTOs;
using PousadaApi.Application.Exceptions;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class MetricasService : IMetricasService
{
    private readonly IPousadaRepository _pousadaRepository;
    private readonly IQuartoRepository _quartoRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly ICurrentUserService _currentUser;

    public MetricasService(
        IPousadaRepository pousadaRepository,
        IQuartoRepository quartoRepository,
        IReservaRepository reservaRepository,
        ICurrentUserService currentUser)
    {
        _pousadaRepository = pousadaRepository;
        _quartoRepository = quartoRepository;
        _reservaRepository = reservaRepository;
        _currentUser = currentUser;
    }

    public async Task<MetricasReadDto> ObterAsync(int pousadaId, DateTime de, DateTime ate, CancellationToken cancellationToken = default)
    {
        de = ToUtc(de);
        ate = ToUtc(ate);

        if (de >= ate)
            throw new InvalidOperationException("A data final deve ser posterior à data inicial.");

        var pertence = await _pousadaRepository.PertenceAoUsuarioAsync(pousadaId, _currentUser.UserId, cancellationToken);
        if (!pertence)
            throw new AcessoNegadoException();

        var quartos = (await _quartoRepository.ListarPorUsuarioAsync(_currentUser.UserId, pousadaId, cancellationToken)).ToList();
        var reservas = (await _reservaRepository.ListarConfirmadasPorPousadaNoPeriodoAsync(
            _currentUser.UserId, pousadaId, de, ate, cancellationToken)).ToList();

        var totalDias = (ate.Date - de.Date).TotalDays;
        if (totalDias < 1) totalDias = 1;

        var quartoDias = quartos.Count * totalDias;
        var diasOcupados = reservas.Sum(r =>
        {
            var inicio = r.DataEntrada < de ? de : r.DataEntrada;
            var fim = r.DataSaida > ate ? ate : r.DataSaida;
            return Math.Max(0, (fim.Date - inicio.Date).TotalDays);
        });

        var taxaOcupacao = quartoDias > 0 ? (decimal)(diasOcupados / quartoDias * 100) : 0;

        return new MetricasReadDto
        {
            PousadaId = pousadaId,
            De = de,
            Ate = ate,
            TotalQuartos = quartos.Count,
            TotalReservas = reservas.Count,
            TaxaOcupacaoPercentual = Math.Round(taxaOcupacao, 2),
            FaturamentoTotal = reservas.Sum(r => r.ValorTotal),
            HospedesUnicos = reservas.Select(r => r.HospedeId).Distinct().Count()
        };
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}
