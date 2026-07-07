using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Order;
public class MarkOrderReadyUseCase : IMarkOrderReadyUseCase {

    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkOrderReadyUseCase> _logger;

    public MarkOrderReadyUseCase(IOrderRepository orderRepo, IUnitOfWork unitOfWork,
        ILogger<MarkOrderReadyUseCase> logger) {
        _orderRepo = orderRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var order = await _orderRepo.GetWithItemsAsync(id, ct)
            ?? throw new NotFoundException("Order", id);

        order.MarkReady();

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order ready. Id={OrderId}", order.Id);

        return order.ToResponse();
    }
}
