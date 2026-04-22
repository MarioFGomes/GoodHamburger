using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.Order;
public class GetOrderByIdUseCase : IGetOrderByIdUseCase {

    private readonly IOrderRepository _orderRepo;

    public GetOrderByIdUseCase(IOrderRepository orderRepo) => _orderRepo = orderRepo;

    public async Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);
        return order.ToResponse();
    }
}
