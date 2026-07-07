using System.Net.Http.Headers;

namespace WebGoodHamburger.Services;

/// <summary>
/// Attaches the cached service-account JWT to every API request.
/// Registered transient (HttpClientFactory composes a new pipeline per
/// client); the token itself lives in the singleton <see cref="ApiTokenCache"/>.
/// </summary>
public class ApiAuthTokenHandler : DelegatingHandler {

    private readonly ApiTokenCache _tokenCache;

    public ApiAuthTokenHandler(ApiTokenCache tokenCache) => _tokenCache = tokenCache;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) {

        var token = await _tokenCache.GetTokenAsync(cancellationToken);
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
