using System.Text.Json;

namespace WebGoodHamburger.Services;

/// <summary>
/// Logs the web app into the API with the configured service account and
/// caches the JWT until shortly before it expires. Singleton: one token is
/// shared by all circuits.
/// </summary>
public class ApiTokenCache {

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<ApiTokenCache> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _accessToken;
    private DateTime _expiresAtUtc;

    public ApiTokenCache(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        JsonSerializerOptions jsonOptions,
        ILogger<ApiTokenCache> logger) {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _jsonOptions = jsonOptions;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(CancellationToken ct = default) {
        // Refresh one minute early so in-flight requests never carry a token
        // that expires mid-request.
        if (_accessToken is not null && DateTime.UtcNow < _expiresAtUtc.AddMinutes(-1))
            return _accessToken;

        await _lock.WaitAsync(ct);
        try {
            if (_accessToken is not null && DateTime.UtcNow < _expiresAtUtc.AddMinutes(-1))
                return _accessToken;

            var email = _configuration["ApiSettings:Email"];
            var password = _configuration["ApiSettings:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
                _logger.LogWarning("ApiSettings:Email/Password not configured — calling the API without a token.");
                return null;
            }

            // Plain client (no auth handler) to avoid recursion through this cache.
            var client = _httpClientFactory.CreateClient("GoodHamburgerAuth");
            var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password }, _jsonOptions, ct);

            if (!response.IsSuccessStatusCode) {
                _logger.LogError("API login failed with status {Status}.", (int)response.StatusCode);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<Models.ApiResponse<AuthPayload>>(_jsonOptions, ct);
            if (envelope?.Data is null) return null;

            _accessToken = envelope.Data.AccessToken;
            _expiresAtUtc = envelope.Data.ExpiresAtUtc;
            _logger.LogInformation("API token acquired; expires at {ExpiresAtUtc:u}.", _expiresAtUtc);
            return _accessToken;
        } catch (Exception ex) {
            _logger.LogError(ex, "Unable to acquire an API token.");
            return null;
        } finally {
            _lock.Release();
        }
    }

    private sealed class AuthPayload {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
