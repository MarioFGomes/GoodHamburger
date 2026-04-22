
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace GoodHamburger.API;
public static class SwaggerConfiguration {

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services) {
        
       
        
        services.AddSwaggerGen(options => {

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
        });



        return services;
    }
}
