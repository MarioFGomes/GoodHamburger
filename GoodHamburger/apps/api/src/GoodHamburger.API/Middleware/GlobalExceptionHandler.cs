using System.Text.Json;
using FluentValidation;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
                "Response já iniciada — não foi possível escrever ProblemDetails.");
            throw exception;
        }

        var (status, title) = MapStatus(exception);

        if (status >= 500)
            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
        else
            _logger.LogWarning(
                "Falha tratada. Status={Status}, Type={ExceptionType}",
                status, exception.GetType().Name);

        var problem = new ProblemDetails {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError && !_env.IsDevelopment()
                ? "Ocorreu um erro interno. Contate o suporte."
                : exception.Message,
            Type = $"https://httpstatuses.com/{status}",
            Instance = context.Request.Path
        };

      
        if (exception is ValidationException ve) {
            problem.Extensions["errors"] = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (int status, string title) MapStatus(Exception ex) => ex switch {
        ValidationException =>                  (StatusCodes.Status400BadRequest, "Erro de validação."),
        NotFoundException =>                    (StatusCodes.Status404NotFound, "Recurso não encontrado."),
        ResourceAlreadyExists =>                (StatusCodes.Status409Conflict, "Recurso já existe."),
        BusinessRuleException =>                (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada."),
        DomainException =>                      (StatusCodes.Status422UnprocessableEntity, "Regra de domínio violada."),
        UnauthorizedAccessException =>          (StatusCodes.Status401Unauthorized, "Acesso não autorizado."),
       
        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor.")
    };
}