using System.Net.Http.Json;
using System.Text.Json;
using WebGoodHamburger.Models;

namespace WebGoodHamburger.Services;
public class MenuService {
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;
    public MenuService(IHttpClientFactory factory, JsonSerializerOptions json) {
        _http = factory.CreateClient("GoodHamburgerApi");
        _json = json;
    }

    public async Task<ApiResult<PagedResponse<MenuResponse>>> GetAllAsync(int page = 1, int pageSize = 10) {
        try {
            var result = await _http.GetFromJsonAsync<PagedResponse<MenuResponse>>($"api/v1/menus?page={page}&pageSize={pageSize}", _json);
            return ApiResult<PagedResponse<MenuResponse>>.Success(result!);
        } catch (Exception ex) { return ApiResult<PagedResponse<MenuResponse>>.Failure(ex.Message); }
    }

    public async Task<ApiResult<MenuResponse>> GetByIdAsync(Guid id) {
        try {
            var result = await _http.GetFromJsonAsync<MenuResponse>($"api/v1/menus/{id}", _json);
            return ApiResult<MenuResponse>.Success(result!);
        } catch (Exception ex) { return ApiResult<MenuResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<MenuResponse>> CreateAsync(CreateMenuRequest request) {
        try {
            var response = await _http.PostAsJsonAsync("api/v1/menus", request, _json);
            if (!response.IsSuccessStatusCode)
                return ApiResult<MenuResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<MenuResponse>.Success((await response.Content.ReadFromJsonAsync<MenuResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<MenuResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<MenuResponse>> UpdateAsync(Guid id, UpdateMenuRequest request) {
        try {
            var response = await _http.PutAsJsonAsync($"api/v1/menus/{id}", request, _json);
            if (!response.IsSuccessStatusCode)
                return ApiResult<MenuResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<MenuResponse>.Success((await response.Content.ReadFromJsonAsync<MenuResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<MenuResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<bool>> DeleteAsync(Guid id) {
        try {
            var response = await _http.DeleteAsync($"api/v1/menus/{id}");
            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
        } catch (Exception ex) { return ApiResult<bool>.Failure(ex.Message); }
    }
}
