using System.Net.Http.Headers;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Infrastructure.Integrations;

public sealed class IcalFeedHttpClient : IIcalFeedClient
{
    private readonly HttpClient _http;

    public IcalFeedHttpClient(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/calendar"));
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
    }

    public async Task<string> BaixarAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
