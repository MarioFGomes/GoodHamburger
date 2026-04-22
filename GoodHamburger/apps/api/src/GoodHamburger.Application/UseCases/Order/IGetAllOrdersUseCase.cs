using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Order;
public interface IGetAllOrdersUseCase {
    Task<PagedResponse<OrderResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default);
}
