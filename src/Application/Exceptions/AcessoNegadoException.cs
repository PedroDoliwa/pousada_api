namespace PousadaApi.Application.Exceptions;

public sealed class AcessoNegadoException : Exception
{
    public AcessoNegadoException()
        : base("Recurso não encontrado.")
    {
    }
}
