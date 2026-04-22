using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.Menu;
public class GetMenuByIdUseCase : IGetMenuByIdUseCase {

    private readonly IMenuRepository _menuRepo;

    public GetMenuByIdUseCase(IMenuRepository menuRepo) => _menuRepo = menuRepo;

    public async Task<MenuResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var menu = await _menuRepo.GetOneAsync(m => m.Id == id, ct);
        if (menu is null) throw new NotFoundException("Menu", id);
        return menu.ToResponse();
    }
}
