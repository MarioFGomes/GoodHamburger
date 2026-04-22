using GoodHamburger.Domain.Repositories;
using GoodHamburger.Infrastructure.DataAcess;
using GoodHamburger.Infrastructure.DataAcess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GoodHamburger.Infrastructure;
public static class Bootstrapper {
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

        var useInMemory = configurationManager.GetValue<bool>("Configurations:InMemoryDataBase");

        if (useInMemory) {
            services.AddDbContext<GoodHamburgerContext>(options => options
                .UseInMemoryDatabase("GoodHamburgerInMemory")
                .UseLazyLoadingProxies());
            return;
        }

        var conectionString = configurationManager.GetConnectionString("SQLServer")
            ?? throw new InvalidOperationException("Connection string 'SQLServer' não encontrada.");

        services.AddDbContext<GoodHamburgerContext>(dbContextOptions => {
            dbContextOptions.UseSqlServer(conectionString);
            dbContextOptions.UseLazyLoadingProxies();
        });

            
        


    }
}