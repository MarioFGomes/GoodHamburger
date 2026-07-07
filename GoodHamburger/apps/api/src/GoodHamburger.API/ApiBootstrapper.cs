using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Asp.Versioning;
using GoodHamburger.API.Filters;
using GoodHamburger.Application.DTOs.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace GoodHamburger.API;
public static class ApiBootstrapper {

    public const string CorsPolicyName = "AllowFrontend";
    public const string AuthRateLimitPolicy = "auth";

    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration) {

        services.AddControllers(options => {
            options.Filters.Add<ValidationFilter>();
        })
        .ConfigureApiBehaviorOptions(options => {
            // Model-binding failures (malformed JSON, null where a value is
            // required) must use the same envelope as every other error.
            options.InvalidModelStateResponseFactory = context => {
                var errors = context.ModelState
                    .Where(kv => kv.Value?.Errors.Count > 0)
                    .SelectMany(kv => kv.Value!.Errors.Select(e =>
                        string.IsNullOrEmpty(kv.Key) ? e.ErrorMessage : $"{kv.Key}: {e.ErrorMessage}"))
                    .ToList();

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                    ApiResponse<object>.Fail("Validation failed.", StatusCodes.Status400BadRequest,
                        errors, context.HttpContext.TraceIdentifier));
            };
        })
        .AddJsonOptions(options => ApiJsonOptions.Apply(options.JsonSerializerOptions));

        services.AddProblemDetails();

        services.AddApiVersioning(options => {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options => {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "https://localhost:7162" };

        services.AddCors(options => {
            options.AddPolicy(CorsPolicyName, policy => {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration) {

        // Fail fast: a missing or weak signing key must abort startup, not
        // silently issue forgeable tokens.
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
            throw new InvalidOperationException(
                "Jwt:Key is missing or shorter than 32 characters. " +
                "Set it in appsettings or via the Jwt__Key environment variable.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "GoodHamburger.API",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "GoodHamburger.Clients",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                };

                // 401/403 must use the same ApiResponse envelope as every
                // other error, so consumers parse one single contract.
                options.Events = new JwtBearerEvents {
                    OnChallenge = context => {
                        context.HandleResponse();
                        return WriteEnvelopeAsync(context.Response, context.HttpContext,
                            StatusCodes.Status401Unauthorized,
                            "Authentication is required to access this resource.");
                    },
                    OnForbidden = context =>
                        WriteEnvelopeAsync(context.Response, context.HttpContext,
                            StatusCodes.Status403Forbidden,
                            "You do not have permission to perform this action."),
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services) {
        services.AddRateLimiter(options => {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Login/registration are brute-force targets: 10 attempts per
            // minute per client IP.
            options.AddPolicy(AuthRateLimitPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    }));

            options.OnRejected = (context, ct) => {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask(WriteEnvelopeAsync(context.HttpContext.Response, context.HttpContext,
                    StatusCodes.Status429TooManyRequests,
                    "Too many requests. Try again in a minute."));
            };
        });
        return services;
    }

    private static Task WriteEnvelopeAsync(HttpResponse response, HttpContext httpContext, int statusCode, string message) {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        var envelope = ApiResponse<object>.Fail(message, statusCode, traceId: httpContext.TraceIdentifier);
        return response.WriteAsync(JsonSerializer.Serialize(envelope, ApiJsonOptions.Default));
    }
}
