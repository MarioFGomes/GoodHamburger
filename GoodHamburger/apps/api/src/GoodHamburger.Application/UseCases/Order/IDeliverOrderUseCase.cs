using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Order;
public interface IDeliverOrderUseCase {
    Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default);
}
