using FluentValidation;
using GoodHamburger.Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Filters;
public class ValidationFilter : IAsyncActionFilter {

    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next) {

        foreach (var argument in context.ActionArguments) {
            if (argument.Value is null) continue;

            var argumentType = argument.Value.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator is null) continue;

            var validationContext = new ValidationContext<object>(argument.Value);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid) {
                var errors = result.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                context.Result = new BadRequestObjectResult(
                    ApiResponse<object>.Fail("Validation failed.", StatusCodes.Status400BadRequest,
                        errors, context.HttpContext.TraceIdentifier));
                return;
            }
        }

        await next();
    }
}
