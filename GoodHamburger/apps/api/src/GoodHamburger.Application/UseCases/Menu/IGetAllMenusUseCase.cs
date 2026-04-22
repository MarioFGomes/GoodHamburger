using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Menu;
public interface IGetAllMenusUseCase {
    Task<PagedResponse<MenuResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default);
}
