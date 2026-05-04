using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Domain.Entities;

public class Quarto
{
    public int Id { get; set; }

    [Required]
    public int PousadaId { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroOuNome { get; set; } = string.Empty;

    [Range(1, 20)]
    public int Capacidade { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ValorDiaria { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Disponivel";

    public Pousada? Pousada { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
