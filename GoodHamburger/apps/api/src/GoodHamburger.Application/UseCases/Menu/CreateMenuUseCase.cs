using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Menu;
public class CreateMenuUseCase : ICreateMenuUseCase {

    private readonly IMenuRepository _menuRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMenuUseCase> _logger;

    public CreateMenuUseCase(IMenuRepository menuRepo, IUnitOfWork unitOfWork,
        ILogger<CreateMenuUseCase> logger) {
        _menuRepo = menuRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MenuResponse> ExecuteAsync(CreateMenuRequest request, CancellationToken ct = default) {

        var nameInUse = await _menuRepo.AnyAsync(m => m.Name == request.Name, ct);
        if (nameInUse) {
            _logger.LogWarning("Attempt to create a menu with an existing name. Name={Name}", request.Name);
            throw new ResourceAlreadyExists("Menu", request.Name);
        }

        var menu = request.ToDomain();

        await _menuRepo.AddOneAsync(menu, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Menu created. Id={MenuId}, Name={Name}", menu.Id, menu.Name);

        return menu.ToResponse();
    }
}
