using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Menu;
public interface IUpdateMenuUseCase {
    Task<MenuResponse> ExecuteAsync(UpdateMenuRequest request, CancellationToken ct = default);
}
