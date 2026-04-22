using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.SideDishes;
public interface IGetSideDishByIdUseCase {
    Task<SideDishesResponse> ExecuteAsync(Guid id, CancellationToken ct = default);
}
