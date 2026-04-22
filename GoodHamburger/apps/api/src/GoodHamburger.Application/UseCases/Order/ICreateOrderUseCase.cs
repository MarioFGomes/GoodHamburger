using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Order;
public interface ICreateOrderUseCase {
    Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken ct = default);
}
