using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class DeliverOrderUseCase : IDeliverOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeliverOrderUseCase> _logger;

    public DeliverOrderUseCase(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        ILogger<DeliverOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);

        order.Deliver();

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order delivered. Id={OrderId}", order.Id);

        return order.ToResponse();
    }
}
