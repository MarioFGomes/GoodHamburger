using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Menu;
public class UpdateMenuUseCase : IUpdateMenuUseCase {

    private readonly IMenuRepository _menuRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMenuUseCase> _logger;

    public UpdateMenuUseCase(IMenuRepository menuRepo, IUnitOfWork unitOfWork,
        ILogger<UpdateMenuUseCase> logger) {
        _menuRepo = menuRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MenuResponse> ExecuteAsync(UpdateMenuRequest request, CancellationToken ct = default) {

        var menu = await _menuRepo.GetOneAsync(m => m.Id == request.Id, ct)
            ?? throw new NotFoundException("Menu", request.Id);

        if (menu.Name != request.Name) {
            var nameInUse = await _menuRepo.AnyAsync(
                m => m.Name == request.Name && m.Id != request.Id, ct);
            if (nameInUse)
                throw new ResourceAlreadyExists("Menu", request.Name);
        }

        var menuToUpdate = request.ToDomain();

        await _menuRepo.ReplaceOneAsync(m => m.Id == menu.Id, menuToUpdate, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Menu updated. Id={MenuId}", menu.Id);

        return menuToUpdate.ToResponse();
    }
}
