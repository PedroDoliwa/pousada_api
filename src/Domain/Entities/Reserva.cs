using System.ComponentModel.DataAnnotations;
using PousadaApi.Domain.Constants;

namespace PousadaApi.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    public int QuartoId { get; set; }

    [Required]
    public int HospedeId { get; set; }

    [Required]
    public DateTime DataEntrada { get; set; }

    [Required]
    public DateTime DataSaida { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = ReservaStatus.Confirmada;

    [Range(0, double.MaxValue)]
    public decimal ValorTotal { get; set; }

    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Required]
    [StringLength(50)]
    public string Origem { get; set; } = ReservaOrigens.Manual;

    [StringLength(500)]
    public string? UidExterno { get; set; }

    public int? CalendarioExternoId { get; set; }

    [StringLength(500)]
    public string? TituloExterno { get; set; }

    public DateTime? SincronizadoEm { get; set; }

    public Quarto? Quarto { get; set; }

    public Hospede? Hospede { get; set; }

    public CalendarioExterno? CalendarioExterno { get; set; }
}
