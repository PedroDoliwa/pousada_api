using System.ComponentModel.DataAnnotations;

namespace PousadaApi.Domain.Entities;

public class Pousada
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required]
    [StringLength(250)]
    public string Endereco { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;

    // Relacionamento com Usuario (gerenciador)
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    // Relacionamento com Quartos
    public ICollection<Quarto> Quartos { get; set; } = new List<Quarto>();
}
