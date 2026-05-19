namespace PousadaApi.Application.DTOs;

public class VerificarDisponibilidadeDto
{
    public int QuartoId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public int? ReservaIdIgnorar { get; set; }
}

public class VerificarDisponibilidadeResultDto
{
    public bool Disponivel { get; set; }
}
