using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GoodHamburger.API;
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions> {

    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options) {
        foreach (var description in _provider.ApiVersionDescriptions) {
            options.SwaggerDoc(
                description.GroupName,           
                CreateOpenApiInfo(description));
        }
    }

    private static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description) {
        var info = new OpenApiInfo {
            Title = "Good Hamburger API",
            Version = description.ApiVersion.ToString(),
            Description = "API RESTful para sistema de pedidos.",
            Contact = new OpenApiContact {
                Name = "Mário Gomes",
                Email = "marioferreiragomes333@gmail.com"
            }
        };

        if (description.IsDeprecated)
            info.Description += " [DEPRECATED] Esta versão está obsoleta.";

        return info;
    }
}
