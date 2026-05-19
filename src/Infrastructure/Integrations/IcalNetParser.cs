using Ical.Net;
using Ical.Net.DataTypes;
using PousadaApi.Application.DTOs;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Infrastructure.Integrations;

public sealed class IcalNetParser : IIcalParser
{
    public IReadOnlyList<IcalEventoDto> Parse(string icsContent)
    {
        var calendar = Calendar.Load(icsContent);
        var eventos = new List<IcalEventoDto>();

        foreach (var evt in calendar.Events)
        {
            if (string.IsNullOrWhiteSpace(evt.Uid))
                continue;

            var inicio = ObterUtc(evt.Start);
            var fim = evt.End is not null ? ObterUtc(evt.End) : inicio.AddDays(1);

            if (fim <= inicio)
                fim = inicio.AddDays(1);

            var status = evt.Properties.Get<string>("STATUS") ?? string.Empty;

            eventos.Add(new IcalEventoDto
            {
                Uid = evt.Uid.Trim(),
                Titulo = evt.Summary,
                DataInicio = inicio,
                DataFim = fim,
                Cancelado = status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase)
            });
        }

        return eventos;
    }

    private static DateTime ObterUtc(IDateTime? value)
    {
        if (value is null)
            return DateTime.UtcNow;

        return value.AsDateTimeOffset.ToUniversalTime().UtcDateTime;
    }
}
