using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Menu;
public class DeleteMenuUseCase : IDeleteMenuUseCase {

    private readonly IMenuRepository _menuRepo;
    private readonly IOrderItemRepository _orderItemRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuUseCase> _logger;

    public DeleteMenuUseCase(IMenuRepository menuRepo, IOrderItemRepository orderItemRepo,
        IUnitOfWork unitOfWork, ILogger<DeleteMenuUseCase> logger) {
        _menuRepo = menuRepo;
        _orderItemRepo = orderItemRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
        var menu = await _menuRepo.GetOneAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("Menu", id);

        var inUse = await _orderItemRepo.AnyAsync(i => i.MenuId == id, ct);
        if (inUse)
            throw new BusinessRuleException("Menu is referenced by orders and cannot be deleted.");

        await _menuRepo.DeleteAsync(m => m.Id == menu.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Menu deleted. Id={MenuId}", id);
    }
}
