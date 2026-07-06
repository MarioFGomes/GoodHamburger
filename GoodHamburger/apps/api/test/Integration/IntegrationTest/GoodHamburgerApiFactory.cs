using GoodHamburger.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

/// <summary>
/// Boots the real API pipeline (routing, filters, middleware, DI) backed by
/// the EF Core InMemory provider, so tests exercise the HTTP contract
/// end-to-end without an external database.
/// </summary>
public class GoodHamburgerApiFactory : WebApplicationFactory<GoodHamburger.API.Program> {

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services => {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<GoodHamburgerContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // One database per factory instance, stable across requests.
            var databaseName = $"GoodHamburgerIntegrationTests-{Guid.NewGuid()}";
            services.AddDbContext<GoodHamburgerContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        });
    }
}
