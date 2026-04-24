using Asp.Versioning.ApiExplorer;
using GoodHamburger.API.Middleware;
using GoodHamburger.Application;
using GoodHamburger.Infrastructure;

namespace GoodHamburger.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services
                   .AddInfrastructure(builder.Configuration)
                   .AddApplication();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApiLayer();
            builder.Services.AddSwaggerConfiguration();

            var app = builder.Build();

            app.Services.MigrateDatabase();

            app.UseMiddleware<GlobalExceptionHandler>();

            // Configure the HTTP request pipeline.
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

            app.UseAuthorization();

            app.UseCors(ApiBootstrapper.CorsPolicyName);

            app.MapControllers();

            app.Run();
        }
    }
}