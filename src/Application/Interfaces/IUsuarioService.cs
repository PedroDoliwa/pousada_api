using PousadaApi.Application.DTOs;
using PousadaApi.Domain.Entities;

namespace PousadaApi.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioReadDto> ObterPerfilAsync(int userId, CancellationToken cancellationToken = default);
    Task<Usuario> AtualizarNomeAsync(int userId, string nome, CancellationToken cancellationToken = default);
    Task AlterarSenhaAsync(int userId, string senhaAtual, string senhaNova, CancellationToken cancellationToken = default);
    Task SalvarFotoAsync(int userId, byte[] bytes, string contentType, CancellationToken cancellationToken = default);
    Task<(byte[]? Bytes, string? ContentType)> ObterFotoAsync(int userId, CancellationToken cancellationToken = default);
    Task RemoverFotoAsync(int userId, CancellationToken cancellationToken = default);
}
