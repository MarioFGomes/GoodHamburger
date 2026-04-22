using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.SideDishes;
public interface IUpdateSideDishesUseCase {
    Task<SideDishesResponse> ExecuteAsync(UpdateSideDishesRequest request, CancellationToken ct = default);
}
