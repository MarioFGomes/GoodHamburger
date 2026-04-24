using System.Text.Json;

namespace WebGoodHamburger.Services;
public static class ApiErrorParser {
    public static string Extract(string raw) {
        try {
            var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("detail", out var detail) && detail.GetString() is { } d)
                return d;
            if (doc.RootElement.TryGetProperty("title", out var title) && title.GetString() is { } t)
                return t;
        } catch { }
        return raw;
    }
}
