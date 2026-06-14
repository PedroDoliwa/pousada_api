namespace PousadaApi.Application.Interfaces;

public interface IRecuperacaoSenhaService
{
    Task SolicitarPorUsuarioAsync(int userId, CancellationToken cancellationToken = default);
    Task SolicitarPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task RedefinirAsync(string token, string novaSenha, CancellationToken cancellationToken = default);
}
