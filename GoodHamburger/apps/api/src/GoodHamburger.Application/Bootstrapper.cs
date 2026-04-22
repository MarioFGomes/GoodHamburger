using GoodHamburger.Application.UseCases.Customer;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace GoodHamburger.Application;
public static class ApplicationBootstrapper {

    public static IServiceCollection AddApplication(this IServiceCollection services) {
        
        services.AddScoped<ICreateCustomerUseCase, CreateCustomerUseCase>();
      

       services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}