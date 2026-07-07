namespace WebGoodHamburger.Models;

/// <summary>
/// Envelope padrão de resposta da API GoodHamburger.
/// </summary>
public class ApiResponse<T> {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }
    public T? Data { get; set; }
}
