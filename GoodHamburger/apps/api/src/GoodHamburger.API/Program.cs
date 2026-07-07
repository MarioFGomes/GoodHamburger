using Asp.Versioning.ApiExplorer;
using GoodHamburger.API.Middleware;
using GoodHamburger.Application;
using GoodHamburger.Infrastructure;
using GoodHamburger.Infrastructure.DataAccess;
using Serilog;

namespace GoodHamburger.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Structured logging to console + rolling files (config-driven, so
            // adding a sink like Seq/Elastic/Datadog is a config change only).
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

            builder.Services
                   .AddInfrastructure(builder.Configuration)
                   .AddApplication();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiLayer(builder.Configuration);
            builder.Services.AddApiAuthentication(builder.Configuration);
            builder.Services.AddApiRateLimiting();
            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddHealthChecks()
                   .AddDbContextCheck<GoodHamburgerContext>("database");

            var app = builder.Build();

            app.Services.InitializeDatabase();

            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseSerilogRequestLogging();

            // Minimal security headers for an API surface.
            app.Use(async (context, next) => {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Cache-Control"] = "no-store";
                await next();
            });

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions) {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"Good Hamburger API {description.GroupName.ToUpperInvariant()}");
                    }
                });
            }

            app.UseHttpsRedirection();

            app.UseCors(ApiBootstrapper.CorsPolicyName);

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
