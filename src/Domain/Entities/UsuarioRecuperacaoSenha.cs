namespace PousadaApi.Domain.Entities;

public class UsuarioRecuperacaoSenha
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public DateTime? UsadoEm { get; set; }
}
