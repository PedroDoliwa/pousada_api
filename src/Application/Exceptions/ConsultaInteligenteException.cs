namespace PousadaApi.Application.Exceptions;

public sealed class ConsultaInteligenteException : Exception
{
    public ConsultaInteligenteException(string message)
        : base(message)
    {
    }

    public ConsultaInteligenteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
