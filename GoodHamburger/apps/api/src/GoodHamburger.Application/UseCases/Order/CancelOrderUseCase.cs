using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class CancelOrderUseCase : ICancelOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelOrderUseCase> _logger;

    public CancelOrderUseCase(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        ILogger<CancelOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order cancelled. Id={OrderId}", order.Id);

        return order.ToResponse();
    }
}
