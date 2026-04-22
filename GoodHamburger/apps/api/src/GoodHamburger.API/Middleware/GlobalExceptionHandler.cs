using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ExceptionHandling;

namespace GoodHamburger.API.Middleware;
public class GlobalExceptionHandler : IExceptionHandler {

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) {
        _logger = logger;
    }

    public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
