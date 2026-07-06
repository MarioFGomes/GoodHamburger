using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.SideDishes;
public class DeleteSideDishesUseCase : IDeleteSideDishesUseCase {

    private readonly ISideDishesRepository _sideDishRepo;
    private readonly IOrderSideDishesRepository _orderSideDishRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSideDishesUseCase> _logger;

    public DeleteSideDishesUseCase(ISideDishesRepository sideDishRepo,
        IOrderSideDishesRepository orderSideDishRepo, IUnitOfWork unitOfWork,
        ILogger<DeleteSideDishesUseCase> logger) {
        _sideDishRepo = sideDishRepo;
        _orderSideDishRepo = orderSideDishRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
        var sideDish = await _sideDishRepo.GetOneAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("SideDish", id);

        var inUse = await _orderSideDishRepo.AnyAsync(s => s.SideDishesId == id, ct);
        if (inUse)
            throw new BusinessRuleException("Side dish is referenced by orders and cannot be deleted.");

        await _sideDishRepo.DeleteAsync(s => s.Id == sideDish.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("SideDish deleted. Id={SideDishId}", id);
    }
}
