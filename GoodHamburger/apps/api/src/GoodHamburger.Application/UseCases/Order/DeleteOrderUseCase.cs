using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class DeleteOrderUseCase : IDeleteOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IOrderItemRepository _itemRepo;
    private readonly IOrderSideDishesRepository _sideDishRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteOrderUseCase> _logger;

    public DeleteOrderUseCase(
        IOrderRepository orderRepo,
        IOrderItemRepository itemRepo,
        IOrderSideDishesRepository sideDishRepo,
        IUnitOfWork unitOfWork,
        ILogger<DeleteOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _itemRepo = itemRepo;
        _sideDishRepo = sideDishRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);

        if (order.Status != OrderStatus.PENDING && order.Status != OrderStatus.CANCELLED)
            throw new BusinessRuleException("Only PENDING or CANCELLED orders can be deleted.");

        await _unitOfWork.BeginTransactionAsync(ct);
        try {
            foreach (var item in order.OrderItems)
                await _sideDishRepo.DeleteAsync(sd => sd.OrderItemId == item.Id, ct);

            await _itemRepo.DeleteAsync(i => i.OrderId == order.Id, ct);
            await _orderRepo.DeleteAsync(o => o.Id == order.Id, ct);

            await _unitOfWork.CommitAsync(ct);
        } catch {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }

        _logger.LogInformation("Order deleted. Id={OrderId}", id);
    }
}
