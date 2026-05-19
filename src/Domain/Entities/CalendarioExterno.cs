using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Domain.Entities;

public class CalendarioExterno
{
    public int Id { get; set; }

    [Required]
    public int QuartoId { get; set; }

    [Required]
    [StringLength(50)]
    public string Canal { get; set; } = "Outro";

    [Required]
    [StringLength(2000)]
    public string UrlImportacao { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public DateTime? UltimaSincronizacao { get; set; }

    [StringLength(2000)]
    public string? UltimoErro { get; set; }

    public Quarto? Quarto { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
