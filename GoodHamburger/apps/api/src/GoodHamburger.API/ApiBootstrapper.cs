using GoodHamburger.API.Filters;
using System.Text.Json.Serialization;
using System.Text.Json;
using Asp.Versioning;

namespace GoodHamburger.API; 
public static class ApiBootstrapper {

    public const string CorsPolicyName = "AllowFrontend";

    public static IServiceCollection AddApiLayer(this IServiceCollection services) {

        services.AddControllers(options => {
            options.Filters.Add<ValidationFilter>();
        })
        .AddJsonOptions(options => {
            options.JsonSerializerOptions.PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddProblemDetails();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfiguration();


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

        services.AddSwaggerConfiguration();

        services.AddCors(options => {
            options.AddPolicy(CorsPolicyName, policy => {
                policy.WithOrigins(
                        "https://localhost:7162/",      
                        "https://meuapp.com")         
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
