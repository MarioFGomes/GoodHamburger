using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Menu;
public class DeleteMenuUseCase : IDeleteMenuUseCase {

    private readonly IMenuRepository _menuRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuUseCase> _logger;

    public DeleteMenuUseCase(IMenuRepository menuRepo, IUnitOfWork unitOfWork,
        ILogger<DeleteMenuUseCase> logger) {
        _menuRepo = menuRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
        var menu = await _menuRepo.GetOneAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("Menu", id);

        await _menuRepo.DeleteAsync(m => m.Id == menu.Id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Menu deleted. Id={MenuId}", id);
    }
}
