using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class CreateOrderUseCase : ICreateOrderUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IMenuRepository _menuRepo;
    private readonly ISideDishesRepository _sideDishRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderUseCase> _logger;

    public CreateOrderUseCase(
        IOrderRepository orderRepo,
        ICustomerRepository customerRepo,
        IMenuRepository menuRepo,
        ISideDishesRepository sideDishRepo,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderUseCase> logger) {
        _orderRepo = orderRepo;
        _customerRepo = customerRepo;
        _menuRepo = menuRepo;
        _sideDishRepo = sideDishRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, string? idempotencyKey = null, CancellationToken ct = default) {

        // Idempotency: a retry with the same key returns the already-created
        // order instead of charging the customer twice. A concurrent duplicate
        // that slips past this check hits the unique index and becomes a 409.
        if (!string.IsNullOrWhiteSpace(idempotencyKey)) {
            var existing = await _orderRepo.GetByIdempotencyKeyAsync(idempotencyKey, ct);
            if (existing is not null) {
                _logger.LogInformation(
                    "Order creation replayed via idempotency key. OrderId={OrderId}", existing.Id);
                return existing.ToResponse();
            }
        }

        var customerExists = await _customerRepo.AnyAsync(c => c.Id == request.CustomerId, ct);
        if (!customerExists)
            throw new NotFoundException("Customer", request.CustomerId);

        var menu = await _menuRepo.GetOneAsync(m => m.Id == request.MenuId, ct)
            ?? throw new NotFoundException("Menu", request.MenuId);

        if (!menu.IsAvailable)
            throw new BusinessRuleException($"Menu '{menu.Name}' is not available.");

        var sideDishes = new List<(Guid Id, SideDishCategory Category, decimal Price)>();
        foreach (var sideDishId in request.SideDishIds ?? new List<Guid>()) {
            var sideDish = await _sideDishRepo.GetOneAsync(s => s.Id == sideDishId, ct)
                ?? throw new NotFoundException("SideDish", sideDishId);

            if (!sideDish.IsAvailable)
                throw new BusinessRuleException($"Side dish '{sideDish.Name}' is not available.");

            sideDishes.Add((sideDish.Id, sideDish.Category, sideDish.Price.Amount));
        }

        var orderNumber = await _orderRepo.NextOrderNumberAsync(ct);
        var order = new Domain.Entities.Order(request.CustomerId, orderNumber, idempotencyKey);

        order.AddSandwich(menu.Id, menu.Price.Amount);

        foreach (var sideDish in sideDishes)
            order.AddSideDish(sideDish.Id, sideDish.Category, sideDish.Price);

        await _unitOfWork.BeginTransactionAsync(ct);
        try {
            await _orderRepo.AddOneAsync(order, ct);
            await _unitOfWork.CommitAsync(ct);
        } catch {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }

        _logger.LogInformation("Order created. Id={OrderId}, OrderNumber={OrderNumber}", order.Id, order.OrderNumber);

        return order.ToResponse();
    }
}
