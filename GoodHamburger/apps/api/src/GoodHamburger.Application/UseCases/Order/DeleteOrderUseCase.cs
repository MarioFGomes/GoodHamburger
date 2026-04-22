using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class DeleteOrderUseCase : IDeleteOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrderUseCase> _logger;

    public DeleteOrderUseCase(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        ILogger<DeleteOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetOneAsync(o => o.Id == id, ct)
            ?? throw new NotFoundException("Order", id);

        if (order.Status != OrderStatus.PENDING && order.Status != OrderStatus.CANCELLED)
            throw new BusinessRuleException("Only PENDING or CANCELLED orders can be deleted.");

        await _orderRepo.DeleteAsync(o => o.Id == order.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order deleted. Id={OrderId}", id);
    }
}
