using System.Net.Http.Json;
using System.Text.Json;
using WebGoodHamburger.Models;

namespace WebGoodHamburger.Services;
public class OrderService {
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;
    public OrderService(IHttpClientFactory factory, JsonSerializerOptions json) {
        _http = factory.CreateClient("GoodHamburgerApi");
        _json = json;
    }

    public async Task<ApiResult<PagedResponse<OrderResponse>>> GetAllAsync(int page = 1, int pageSize = 10) {
        try {
            var result = await _http.GetFromJsonAsync<PagedResponse<OrderResponse>>($"api/v1/orders?page={page}&pageSize={pageSize}", _json);
            return ApiResult<PagedResponse<OrderResponse>>.Success(result!);
        } catch (Exception ex) { return ApiResult<PagedResponse<OrderResponse>>.Failure(ex.Message); }
    }

    public async Task<ApiResult<OrderResponse>> GetByIdAsync(Guid id) {
        try {
            var result = await _http.GetFromJsonAsync<OrderResponse>($"api/v1/orders/{id}", _json);
            return ApiResult<OrderResponse>.Success(result!);
        } catch (Exception ex) { return ApiResult<OrderResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<OrderResponse>> CreateAsync(CreateOrderRequest request) {
        try {
            var response = await _http.PostAsJsonAsync("api/v1/orders", request, _json);
            if (!response.IsSuccessStatusCode)
                return ApiResult<OrderResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<OrderResponse>.Success((await response.Content.ReadFromJsonAsync<OrderResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<OrderResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<OrderResponse>> ConfirmAsync(Guid id) {
        try {
            var response = await _http.PutAsync($"api/v1/orders/{id}/confirm", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult<OrderResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<OrderResponse>.Success((await response.Content.ReadFromJsonAsync<OrderResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<OrderResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<OrderResponse>> CancelAsync(Guid id) {
        try {
            var response = await _http.PutAsync($"api/v1/orders/{id}/cancel", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult<OrderResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<OrderResponse>.Success((await response.Content.ReadFromJsonAsync<OrderResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<OrderResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<bool>> DeleteAsync(Guid id) {
        try {
            var response = await _http.DeleteAsync($"api/v1/orders/{id}");
            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
        } catch (Exception ex) { return ApiResult<bool>.Failure(ex.Message); }
    }
}
