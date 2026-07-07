namespace GoodHamburger.Application.DTOs.Responses;
public class ApiResponse<T> {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }

    /// <summary>Correlates an error response with the server-side logs.</summary>
    public string? TraceId { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null, int statusCode = 200) => new() {
        Success = true,
        Message = message,
        StatusCode = statusCode,
        Data = data
    };

    public static ApiResponse<T> Fail(string message, int statusCode, List<string>? errors = null, string? traceId = null) => new() {
        Success = false,
        Message = message,
        StatusCode = statusCode,
        Errors = errors,
        TraceId = traceId
    };
}

public static class ApiResponse {
    /// <summary>Success envelope with no payload (e.g. deletes).</summary>
    public static ApiResponse<object> Ok(string? message = null, int statusCode = 200) => new() {
        Success = true,
        Message = message,
        StatusCode = statusCode
    };
}
