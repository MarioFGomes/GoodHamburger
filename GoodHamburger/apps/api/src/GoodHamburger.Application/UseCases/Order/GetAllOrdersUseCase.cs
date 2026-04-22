using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.Order;
public class GetAllOrdersUseCase : IGetAllOrdersUseCase {

    private readonly IOrderRepository _orderRepo;

    public GetAllOrdersUseCase(IOrderRepository orderRepo) => _orderRepo = orderRepo;

    public async Task<PagedResponse<OrderResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default) {

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var orders = await _orderRepo.GetAllWithItemsAsync(page, pageSize, ct);
        var total = await _orderRepo.CountAsync(ct);

        return new PagedResponse<OrderResponse> {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Items = orders.Select(o => o.ToResponse()).ToList()
        };
    }
}
