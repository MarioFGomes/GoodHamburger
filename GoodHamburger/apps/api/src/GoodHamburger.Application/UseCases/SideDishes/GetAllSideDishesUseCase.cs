using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.SideDishes;
public class GetAllSideDishesUseCase : IGetAllSideDishesUseCase {

    private readonly ISideDishesRepository _sideDishRepo;

    public GetAllSideDishesUseCase(ISideDishesRepository sideDishRepo) => _sideDishRepo = sideDishRepo;

    public async Task<PagedResponse<SideDishesResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default) {

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var sideDishes = await _sideDishRepo.GetAllAsync(ct, page, pageSize);
        var total = await _sideDishRepo.CountAsync(ct);

        return new PagedResponse<SideDishesResponse> {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Items = sideDishes.Select(s => s.ToResponse()).ToList()
        };
    }
}
