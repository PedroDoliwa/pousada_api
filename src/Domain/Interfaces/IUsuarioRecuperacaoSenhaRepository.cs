using PousadaApi.Domain.Entities;

namespace PousadaApi.Domain.Interfaces;

public interface IUsuarioRecuperacaoSenhaRepository
{
    Task AdicionarAsync(UsuarioRecuperacaoSenha recuperacao, CancellationToken cancellationToken = default);
    Task<UsuarioRecuperacaoSenha?> ObterValidoPorTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task MarcarComoUsadoAsync(UsuarioRecuperacaoSenha recuperacao, CancellationToken cancellationToken = default);
}
