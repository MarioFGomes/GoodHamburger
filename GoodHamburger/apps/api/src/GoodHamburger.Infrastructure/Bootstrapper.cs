using GoodHamburger.Application.Auth;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.ValueObjects;
using GoodHamburger.Infrastructure.Auth;
using GoodHamburger.Infrastructure.DataAccess;
using GoodHamburger.Infrastructure.DataAccess.Repositories;
using GoodHamburger.Infrastructure.DataAccess.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure;
public static class Bootstrapper {

    /// <summary>
    /// Migrates (relational) or creates (in-memory) the database, then seeds
    /// the catalog and the admin user. A migration failure aborts startup with
    /// a clear log instead of letting the app run against a broken schema.
    /// </summary>
    public static void InitializeDatabase(this IServiceProvider services) {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GoodHamburgerContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GoodHamburger.Infrastructure.DatabaseInitializer");

        try {
            if (context.Database.IsRelational())
                context.Database.Migrate();
            else
                context.Database.EnsureCreated();
        } catch (Exception ex) {
            logger.LogCritical(ex,
                "Database migration failed. If a unique index migration is involved, " +
                "check for pre-existing duplicate rows before deploying.");
            throw;
        }

        SeedData.EnsureSeeded(context);
        SeedAdminUser(scope.ServiceProvider, context, logger);
    }

    private static void SeedAdminUser(IServiceProvider provider, GoodHamburgerContext context, ILogger logger) {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var email = configuration["Auth:AdminUser:Email"];
        var password = configuration["Auth:AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
            logger.LogWarning("Auth:AdminUser is not configured — no admin user was seeded.");
            return;
        }

        var adminEmail = Email.Create(email);
        if (context.Set<User>().Any(u => u.Email == adminEmail)) return;

        var hasher = provider.GetRequiredService<IPasswordHasher>();
        context.Set<User>().Add(new User("Administrator", adminEmail, hasher.Hash(password), UserRole.ADMIN));
        context.SaveChanges();

        logger.LogInformation("Admin user seeded. Email={Email}", adminEmail.Value);
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configurationManager) {
        AddContext(services, configurationManager);
        AddRepositories(services);
        AddAuthServices(services, configurationManager);
        return services;
    }

    private static void AddAuthServices(IServiceCollection services, IConfiguration configuration) {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();
    }

    private static void AddRepositories(IServiceCollection services) {

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<GoodHamburgerContext>());

        services.AddScoped<ICustomerRepository,        CustomerRepository>()
                .AddScoped<IMenuRepository,            MenuRepository>()
                .AddScoped<IOrderRepository,           OrderRepository>()
                .AddScoped<IOrderItemRepository,       OrderItemRepository>()
                .AddScoped<IOrderSideDishesRepository, OrderSideDishesRepository>()
                .AddScoped<ISideDishesRepository,      SideDishesRepository>()
                .AddScoped<IUserRepository,            UserRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddContext(IServiceCollection services, IConfiguration configurationManager) {

        bool.TryParse(configurationManager["Configurations:InMemoryDataBase"], out var useInMemory);

        if (useInMemory) {
            services.AddDbContext<GoodHamburgerContext>(options => options
                    .UseInMemoryDatabase("GoodHamburgerInMemory"));
            return;
        }

        var connectionString = configurationManager.GetConnectionString("SQLServer")
            ?? throw new InvalidOperationException("Connection string 'SQLServer' was not found.");

        services.AddDbContext<GoodHamburgerContext>(dbContextOptions => {
            dbContextOptions.UseSqlServer(connectionString);
        });
    }
}
