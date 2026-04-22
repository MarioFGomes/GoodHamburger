using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.SideDishes;
public interface IGetAllSideDishesUseCase {
    Task<PagedResponse<SideDishesResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default);
}
