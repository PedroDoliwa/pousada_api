using PousadaApi.Application.DTOs;

namespace PousadaApi.Application.Interfaces;

public interface IIcalParser
{
    IReadOnlyList<IcalEventoDto> Parse(string icsContent);
}
