using GoodHamburger.Application.UseCases.Customer;
using GoodHamburger.Application.UseCases.Menu;
using GoodHamburger.Application.UseCases.Order;
using GoodHamburger.Application.UseCases.SideDishes;
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

        services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>()
                .AddScoped<IGetOrderByIdUseCase, GetOrderByIdUseCase>()
                .AddScoped<IGetAllOrdersUseCase, GetAllOrdersUseCase>()
                .AddScoped<IConfirmOrderUseCase, ConfirmOrderUseCase>()
                .AddScoped<ICancelOrderUseCase, CancelOrderUseCase>()
                .AddScoped<IDeleteOrderUseCase, DeleteOrderUseCase>();

        services.AddScoped<ICreateSideDishesUseCase, CreateSideDishesUseCase>()
                .AddScoped<IGetSideDishByIdUseCase, GetSideDishByIdUseCase>()
                .AddScoped<IGetAllSideDishesUseCase, GetAllSideDishesUseCase>()
                .AddScoped<IUpdateSideDishesUseCase, UpdateSideDishesUseCase>()
                .AddScoped<IDeleteSideDishesUseCase, DeleteSideDishesUseCase>();



        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}