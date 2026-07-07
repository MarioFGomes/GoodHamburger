using System.Text.Json;
using FluentValidation;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.API.Middleware;

public class GlobalExceptionHandler {

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment env) {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) {

        if (context.Response.HasStarted) {
            _logger.LogError(exception,
                "Response has already started — unable to write the error envelope.");
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var (status, detail) = MapStatus(exception);

        if (status >= 500)
            _logger.LogError(exception, "Unhandled error: {Message}", exception.Message);
        else
            _logger.LogWarning(
                "Handled failure. Status={Status}, Type={ExceptionType}",
                status, exception.GetType().Name);

        var message = detail ?? (status == StatusCodes.Status500InternalServerError && !_env.IsDevelopment()
            ? "An internal error occurred. Contact support with the traceId."
            : exception.Message);

        var errors = exception is ValidationException ve
            ? ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList()
            : null;

        var apiResponse = ApiResponse<object>.Fail(message, status, errors, context.TraceIdentifier);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        // Same options as MVC — error payloads can never drift from success payloads.
        var json = JsonSerializer.Serialize(apiResponse, ApiJsonOptions.Default);

        await context.Response.WriteAsync(json);
    }

    private static (int status, string? detail) MapStatus(Exception ex) => ex switch {
        ValidationException =>              (StatusCodes.Status400BadRequest, null),
        NotFoundException =>                (StatusCodes.Status404NotFound, null),
        ResourceAlreadyExists =>            (StatusCodes.Status409Conflict, null),
        BusinessRuleException =>            (StatusCodes.Status422UnprocessableEntity, null),
        DomainException =>                  (StatusCodes.Status422UnprocessableEntity, null),
        InvalidCredentialsException =>      (StatusCodes.Status401Unauthorized, null),
        UnauthorizedAccessException =>      (StatusCodes.Status401Unauthorized, null),

        // Optimistic concurrency: the row changed (or vanished) between read
        // and write. The client should refetch and retry.
        DbUpdateConcurrencyException =>     (StatusCodes.Status409Conflict,
                                             "The resource was modified or removed by another request. Refresh and try again."),

        // Safety net: unique index / FK violations that slipped past the
        // use-case checks (e.g. two concurrent inserts with the same phone).
        DbUpdateException =>                (StatusCodes.Status409Conflict,
                                             "The operation violates a data integrity constraint (duplicate or referenced record)."),

        _ => (StatusCodes.Status500InternalServerError, null)
    };
}
