using System.ComponentModel.DataAnnotations;

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
    public string Status { get; set; } = "Confirmada";

    [Range(0, double.MaxValue)]
    public decimal ValorTotal { get; set; }

    [StringLength(500)]
    public string? Observacoes { get; set; }

    public Quarto? Quarto { get; set; }

    public Hospede? Hospede { get; set; }
}
