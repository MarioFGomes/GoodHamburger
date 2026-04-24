using GoodHamburger.Domain.Repositories;
using GoodHamburger.Infrastructure.DataAcess;
using GoodHamburger.Infrastructure.DataAcess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GoodHamburger.Infrastructure;
public static class Bootstrapper {
    public static void MigrateDatabase(this IServiceProvider services) {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GoodHamburgerContext>();
        if (context.Database.IsRelational())
            context.Database.Migrate();
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configurationManager) {
        AddContext(services, configurationManager);
        AddRepositories(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services) {

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<GoodHamburgerContext>());

        services.AddScoped<ICustomerRepository,        CustomerRepository>()
                .AddScoped<IMenuRepository,            MenuRepository>()
                .AddScoped<IOrderRepository,           OrderRepository>()
                .AddScoped<IOrderItemRepository,       OrderItemRepository>()
                .AddScoped<IOrderSideDishesRepository, OrderSideDishesRepository>()
                .AddScoped<ISideDishesRepository,      SideDishesRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();

    }

    private static void AddContext(IServiceCollection services, IConfiguration configurationManager) {

        bool.TryParse(configurationManager["Configurations:InMemoryDataBase"], out var useInMemory);

        if (useInMemory) {
            services.AddDbContext<GoodHamburgerContext>(options => options
                    .UseInMemoryDatabase("GoodHamburgerInMemory"));
            return;
        }

        var conectionString = configurationManager.GetConnectionString("SQLServer")
            ?? throw new InvalidOperationException("Connection string 'SQLServer' não encontrada.");

        services.AddDbContext<GoodHamburgerContext>(dbContextOptions => {
            dbContextOptions.UseSqlServer(conectionString);
        });

            
        


    }
}