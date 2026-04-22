using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Entities;
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

    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken ct = default) {

        var customerExists = await _customerRepo.AnyAsync(c => c.Id == request.CustomerId, ct);
        if (!customerExists)
            throw new NotFoundException("Customer", request.CustomerId);

        var menu = await _menuRepo.GetOneAsync(m => m.Id == request.MenuId, ct)
            ?? throw new NotFoundException("Menu", request.MenuId);

        var sideDishes = new List<Domain.Entities.SideDishes>();
        foreach (var sideDishId in request.SideDishIds) {
            var sideDish = await _sideDishRepo.GetOneAsync(s => s.Id == sideDishId, ct)
                ?? throw new NotFoundException("SideDish", sideDishId);
            sideDishes.Add(sideDish);
        }

        var orderNumber = await _orderRepo.CountAsync(ct) + 1;
        var order = new Domain.Entities.Order(request.CustomerId, orderNumber);

        order.AddSandwich(menu.Id, menu.Price!.Value);

        foreach (var sideDish in sideDishes)
            order.AddSideDish(sideDish.Id, sideDish.Category, sideDish.Price!.Value);

        await _orderRepo.AddOneAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order created. Id={OrderId}, OrderNumber={OrderNumber}", order.Id, order.OrderNumber);

        return order.ToResponse();
    }
}
