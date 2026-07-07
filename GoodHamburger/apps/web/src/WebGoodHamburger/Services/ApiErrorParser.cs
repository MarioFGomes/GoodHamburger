using System.Text.Json;

namespace WebGoodHamburger.Services;
public static class ApiErrorParser {
    public static string Extract(string raw) {
        try {
            var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            // Contrato ApiResponse: { success, message, errors, statusCode, data }
            if (root.TryGetProperty("message", out var message) && message.GetString() is { } m) {
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array) {
                    var list = errors.EnumerateArray()
                        .Select(e => e.GetString())
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .ToList();
                    if (list.Count > 0)
                        return $"{m} {string.Join(" | ", list)}";
                }
                return m;
            }

            // Fallback: ProblemDetails (respostas antigas / geradas pelo framework)
            if (root.TryGetProperty("detail", out var detail) && detail.GetString() is { } d)
                return d;
            if (root.TryGetProperty("title", out var title) && title.GetString() is { } t)
                return t;
        } catch { }
        return raw;
    }
}
