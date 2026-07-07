using System.Net.Http.Json;
using System.Text.Json;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

/// <summary>
/// Boots the real API pipeline (routing, filters, middleware, auth, DI)
/// backed by the EF Core InMemory provider, so tests exercise the HTTP
/// contract end-to-end without an external database.
/// </summary>
public class GoodHamburgerApiFactory : WebApplicationFactory<GoodHamburger.API.Program> {

    public const string AdminEmail = "admin@goodhamburger.local";
    public const string AdminPassword = "Admin@12345";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private string? _adminToken;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseEnvironment("Development");

        // Let the app itself register the InMemory provider (EF 10 rejects a
        // service provider containing both SqlServer and InMemory services),
        // then swap in a database name unique to this factory instance.
        builder.UseSetting("Configurations:InMemoryDataBase", "true");

        builder.ConfigureServices(services => {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<GoodHamburgerContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            var databaseName = $"GoodHamburgerIntegrationTests-{Guid.NewGuid()}";
            services.AddDbContext<GoodHamburgerContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        });
    }

    /// <summary>
    /// Logs in as the seeded admin exactly once per factory (the auth
    /// endpoints are rate limited — 10/min — so each test must not login again).
    /// </summary>
    public async Task<string> GetAdminTokenAsync(HttpClient client) {
        if (_adminToken is not null) return _adminToken;

        await _tokenLock.WaitAsync();
        try {
            if (_adminToken is not null) return _adminToken;

            var response = await client.PostAsJsonAsync("/api/v1/auth/login",
                new { email = AdminEmail, password = AdminPassword }, JsonOptions);
            response.EnsureSuccessStatusCode();

            var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptions);
            _adminToken = envelope!.Data!.AccessToken;
            return _adminToken;
        } finally {
            _tokenLock.Release();
        }
    }

    public async Task<HttpClient> CreateAdminClientAsync() {
        var client = CreateClient();
        var token = await GetAdminTokenAsync(client);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
