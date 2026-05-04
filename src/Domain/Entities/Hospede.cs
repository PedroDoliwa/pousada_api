using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Domain.Entities;

public class Hospede
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Documento { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
