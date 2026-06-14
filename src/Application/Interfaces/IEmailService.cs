namespace PousadaApi.Application.Interfaces;

public interface IEmailService
{
    Task EnviarRedefinicaoSenhaAsync(string destinatario, string linkRedefinicao, CancellationToken cancellationToken = default);
}
