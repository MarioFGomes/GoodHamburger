namespace WebGoodHamburger.Services;
public class ApiResult<T> {
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public static ApiResult<T> Success(T data) => new() { Data = data, IsSuccess = true };
    public static ApiResult<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
