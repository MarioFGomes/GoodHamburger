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
                "Response já iniciada — não foi possível escrever ProblemDetails.");
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var (status, title, detail) = MapStatus(exception);

        if (status >= 500)
            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
        else
            _logger.LogWarning(
                "Falha tratada. Status={Status}, Type={ExceptionType}",
                status, exception.GetType().Name);

        var message = detail ?? (status == StatusCodes.Status500InternalServerError && !_env.IsDevelopment()
            ? "Ocorreu um erro interno. Contate o suporte."
            : exception.Message);

        var errors = exception is ValidationException ve
            ? ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList()
            : null;

        var apiResponse = ApiResponse<object>.Fail(message, status, errors);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }

    private static (int status, string title, string? detail) MapStatus(Exception ex) => ex switch {
        ValidationException =>                  (StatusCodes.Status400BadRequest, "Erro de validação.", null),
        NotFoundException =>                    (StatusCodes.Status404NotFound, "Recurso não encontrado.", null),
        ResourceAlreadyExists =>                (StatusCodes.Status409Conflict, "Recurso já existe.", null),
        BusinessRuleException =>                (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada.", null),
        DomainException =>                      (StatusCodes.Status422UnprocessableEntity, "Regra de domínio violada.", null),
        UnauthorizedAccessException =>          (StatusCodes.Status401Unauthorized, "Acesso não autorizado.", null),

        // Rede de segurança: violações de índice único/FK que escaparam às
        // verificações dos use cases (ex.: dois inserts concorrentes com o mesmo telefone).
        DbUpdateException =>                    (StatusCodes.Status409Conflict, "Conflito de dados.",
                                                 "A operação viola uma restrição de integridade (registro duplicado ou em uso)."),

        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor.", null)
    };
}
