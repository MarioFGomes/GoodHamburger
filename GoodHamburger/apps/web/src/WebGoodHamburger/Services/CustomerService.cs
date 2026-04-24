using System.Net.Http.Json;
using System.Text.Json;
using WebGoodHamburger.Models;

namespace WebGoodHamburger.Services;
public class CustomerService {
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;
    public CustomerService(IHttpClientFactory factory, JsonSerializerOptions json) {
        _http = factory.CreateClient("GoodHamburgerApi");
        _json = json;
    }

    public async Task<ApiResult<PagedResponse<CustomerResponse>>> GetAllAsync(int page = 1, int pageSize = 10) {
        try {
            var result = await _http.GetFromJsonAsync<PagedResponse<CustomerResponse>>($"api/v1/customers?page={page}&pageSize={pageSize}", _json);
            return ApiResult<PagedResponse<CustomerResponse>>.Success(result!);
        } catch (Exception ex) { return ApiResult<PagedResponse<CustomerResponse>>.Failure(ex.Message); }
    }

    public async Task<ApiResult<CustomerResponse>> GetByIdAsync(Guid id) {
        try {
            var result = await _http.GetFromJsonAsync<CustomerResponse>($"api/v1/customers/{id}", _json);
            return ApiResult<CustomerResponse>.Success(result!);
        } catch (Exception ex) { return ApiResult<CustomerResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request) {
        try {
            var response = await _http.PostAsJsonAsync("api/v1/customers", request, _json);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<CustomerResponse>.Success((await response.Content.ReadFromJsonAsync<CustomerResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<CustomerResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<CustomerResponse>> UpdateAsync(Guid id, UpdateCustomerRequest request) {
        try {
            var response = await _http.PutAsJsonAsync($"api/v1/customers/{id}", request, _json);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerResponse>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
            return ApiResult<CustomerResponse>.Success((await response.Content.ReadFromJsonAsync<CustomerResponse>(_json))!);
        } catch (Exception ex) { return ApiResult<CustomerResponse>.Failure(ex.Message); }
    }

    public async Task<ApiResult<bool>> DeleteAsync(Guid id) {
        try {
            var response = await _http.DeleteAsync($"api/v1/customers/{id}");
            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Failure(ApiErrorParser.Extract(await response.Content.ReadAsStringAsync()));
        } catch (Exception ex) { return ApiResult<bool>.Failure(ex.Message); }
    }
}
