namespace PousadaApi.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string SenhaHash { get; set; }
    public string Perfil { get; set; } // "Admin", "Gerente", "Funcionario"

    public byte[]? Foto { get; set; }
    public string? FotoContentType { get; set; }

    // Relacionamento com Pousada
    public ICollection<Pousada> Pousadas { get; set; } = new List<Pousada>();
}
