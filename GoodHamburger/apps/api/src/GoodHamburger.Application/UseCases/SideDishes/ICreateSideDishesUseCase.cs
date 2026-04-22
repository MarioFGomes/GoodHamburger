using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.SideDishes;
public interface ICreateSideDishesUseCase {
    Task<SideDishesResponse> ExecuteAsync(CreateSideDishesRequest request, CancellationToken ct = default);
}
