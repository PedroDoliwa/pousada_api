using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class IcalExportService : IIcalExportService
{
    private readonly IQuartoRepository _quartoRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly ICurrentUserService _currentUser;

    public IcalExportService(
        IQuartoRepository quartoRepository,
        IReservaRepository reservaRepository,
        ICurrentUserService currentUser)
    {
        _quartoRepository = quartoRepository;
        _reservaRepository = reservaRepository;
        _currentUser = currentUser;
    }

    public async Task<string> GerarCalendarioQuartoAsync(int quartoId, CancellationToken cancellationToken = default)
    {
        var quarto = await _quartoRepository.ObterPorIdEUsuarioAsync(quartoId, _currentUser.UserId, cancellationToken);
        if (quarto is null)
            throw new Exceptions.AcessoNegadoException();

        return await GerarIcsAsync(quartoId, quarto.NumeroOuNome, cancellationToken);
    }

    public async Task<string> GerarCalendarioPorTokenAsync(string tokenExportacao, CancellationToken cancellationToken = default)
    {
        var quarto = await _quartoRepository.ObterPorTokenExportacaoAsync(tokenExportacao, cancellationToken);
        if (quarto is null)
            throw new InvalidOperationException("Token de exportação inválido.");

        return await GerarIcsAsync(quarto.Id, quarto.NumeroOuNome, cancellationToken);
    }

    private async Task<string> GerarIcsAsync(int quartoId, string nomeQuarto, CancellationToken cancellationToken)
    {
        var reservas = await _reservaRepository.ListarAtivasPorQuartoAsync(quartoId, cancellationToken);
        var calendar = new Calendar { Name = $"Pousada API - Quarto {nomeQuarto}" };

        foreach (var reserva in reservas)
        {
            var uid = string.IsNullOrEmpty(reserva.UidExterno)
                ? $"pousada-api-reserva-{reserva.Id}@pousada-api"
                : reserva.UidExterno;

            var evt = new CalendarEvent
            {
                Uid = uid,
                Summary = reserva.TituloExterno ?? $"Reserva #{reserva.Id}",
                Start = new CalDateTime(reserva.DataEntrada),
                End = new CalDateTime(reserva.DataSaida),
                Description = reserva.Observacoes
            };
            calendar.Events.Add(evt);
        }

        return new CalendarSerializer().SerializeToString(calendar);
    }
}
