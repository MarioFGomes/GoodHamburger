using System.Net.Http.Json;
using System.Text.Json;
using WebGoodHamburger.Models;

namespace WebGoodHamburger.Services;
public class SideDishService {
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;
    public SideDishService(IHttpClientFactory factory, JsonSerializerOptions json) {
        _http = factory.CreateClient("GoodHamburgerApi");
        _json = json;
    }

    public async Task<ApiResult<PagedResponse<SideDishesResponse>>> GetAllAsync(int page = 1, int pageSize = 10) {
        try {
            var result = await _http.GetFromJsonAsync<PagedResponse<SideDishesResponse>>($"api/v1/side-dishes?page={page}&pageSize={pageSize}", _json);
            return ApiResult<PagedResponse<SideDishesResponse>>.Success(result!);
        } catch (Exception ex) { return ApiResult<PagedResponse<SideDishesResponse>>.Failure(ex.Message); }
    }

    public async Task<ApiResult<SideDishesResponse>> GetByIdAsync(Guid id) {
        try {
            var result = await _http.GetFromJsonAsync<SideDishesResponse>($"api/v1/side-dishes/{id}", _json);
            return ApiResult<SideDishesResponse>.Success(result!);
        } catch (Exception ex) { return ApiResult<SideDishesResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<SideDishesResponse>> CreateAsync(CreateSideDishesRequest request) {
        try {
            var response = await _http.PostAsJsonAsync("api/v1/side-dishes", request, _json);
            if (!response.IsSuccessStatusCode) return ApiResult<SideDishesResponse>.Failure(await response.Content.ReadAsStringAsync());
            return ApiResult<SideDishesResponse>.Success((await response.Content.ReadFromJsonAsync<SideDishesResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<SideDishesResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<SideDishesResponse>> UpdateAsync(Guid id, UpdateSideDishesRequest request) {
        try {
            var response = await _http.PutAsJsonAsync($"api/v1/side-dishes/{id}", request, _json);
            if (!response.IsSuccessStatusCode) return ApiResult<SideDishesResponse>.Failure(await response.Content.ReadAsStringAsync());
            return ApiResult<SideDishesResponse>.Success((await response.Content.ReadFromJsonAsync<SideDishesResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<SideDishesResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<bool>> DeleteAsync(Guid id) {
        try {
            var response = await _http.DeleteAsync($"api/v1/side-dishes/{id}");
            return response.IsSuccessStatusCode ? ApiResult<bool>.Success(true) : ApiResult<bool>.Failure(await response.Content.ReadAsStringAsync());
        } catch (Exception ex) { return ApiResult<bool>.Failure(ex.Message); }
    }
}
