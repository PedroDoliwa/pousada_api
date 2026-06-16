namespace PousadaApi.Application.Exceptions;

// Mensagem neutra para não confirmar quais e-mails já existem (anti-enumeração).
public sealed class EmailJaCadastradoException : Exception
{
    public EmailJaCadastradoException()
        : base("Não foi possível concluir o cadastro com os dados informados.")
    {
    }
}
