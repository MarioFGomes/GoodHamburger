using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoodHamburger.API;

/// <summary>
/// Single source of truth for JSON serialization. MVC and the exception
/// middleware both use these options, so error and success payloads can
/// never drift apart in casing or enum handling.
/// </summary>
public static class ApiJsonOptions {

    public static readonly JsonSerializerOptions Default = CreateDefault();

    private static JsonSerializerOptions CreateDefault() {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    public static void Apply(JsonSerializerOptions target) {
        target.PropertyNamingPolicy = Default.PropertyNamingPolicy;
        target.ReferenceHandler = Default.ReferenceHandler;
        target.DefaultIgnoreCondition = Default.DefaultIgnoreCondition;
        foreach (var converter in Default.Converters)
            target.Converters.Add(converter);
    }
}
