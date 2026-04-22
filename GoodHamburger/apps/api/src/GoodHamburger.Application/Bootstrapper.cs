using GoodHamburger.Application.UseCases.Customer;
using GoodHamburger.Application.UseCases.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace GoodHamburger.Application;
public static class ApplicationBootstrapper {

    public static IServiceCollection AddApplication(this IServiceCollection services) {

        services.AddScoped<ICreateCustomerUseCase, CreateCustomerUseCase>()
                .AddScoped<IGetCustomerByIdUseCase, GetCustomerByIdUseCase>()
                .AddScoped<IGetAllCustomersUseCase, GetAllCustomersUseCase>()
                .AddScoped<IUpdateCustomerUseCase, UpdateCustomerUseCase>()
                .AddScoped<IDeleteCustomerUseCase, DeleteCustomerUseCase>();

        services.AddScoped<ICreateMenuUseCase, CreateMenuUseCase>()
                .AddScoped<IGetMenuByIdUseCase, GetMenuByIdUseCase>()
                .AddScoped<IGetAllMenusUseCase, GetAllMenusUseCase>()
                .AddScoped<IUpdateMenuUseCase, UpdateMenuUseCase>()
                .AddScoped<IDeleteMenuUseCase, DeleteMenuUseCase>();



        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}