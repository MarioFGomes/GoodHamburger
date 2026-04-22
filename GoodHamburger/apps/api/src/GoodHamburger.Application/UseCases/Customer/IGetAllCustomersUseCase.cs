using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Customer; 
public interface IGetAllCustomersUseCase {
    Task<PagedResponse<CustomerResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default);
}
