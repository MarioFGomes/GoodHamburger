using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.Menu;
public class GetAllMenusUseCase : IGetAllMenusUseCase {

    private readonly IMenuRepository _menuRepo;

    public GetAllMenusUseCase(IMenuRepository menuRepo) => _menuRepo = menuRepo;

    public async Task<PagedResponse<MenuResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default) {

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var menus = await _menuRepo.GetAllAsync(ct, page, pageSize);
        var total = await _menuRepo.CountAsync(ct);

        return new PagedResponse<MenuResponse> {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Items = menus.Select(m => m.ToResponse()).ToList()
        };
    }
}
