using GoodHamburger.Application.UseCases.Customer;
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



        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}