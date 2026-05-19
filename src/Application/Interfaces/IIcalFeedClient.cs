namespace PousadaApi.Application.Interfaces;

public interface IIcalFeedClient
{
    Task<string> BaixarAsync(string url, CancellationToken cancellationToken = default);
}
