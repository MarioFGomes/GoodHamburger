using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class PayOrderUseCase : IPayOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PayOrderUseCase> _logger;

    public PayOrderUseCase(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        ILogger<PayOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);

        order.Pay();

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order paid. Id={OrderId}, Total={Total}", order.Id, order.Total);

        return order.ToResponse();
    }
}
