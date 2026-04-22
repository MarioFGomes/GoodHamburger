using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Menu;
public interface ICreateMenuUseCase {
    Task<MenuResponse> ExecuteAsync(CreateMenuRequest request, CancellationToken ct = default);
}
