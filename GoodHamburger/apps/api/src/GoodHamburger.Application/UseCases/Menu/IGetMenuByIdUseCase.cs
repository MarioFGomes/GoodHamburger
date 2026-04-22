using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Menu;
public interface IGetMenuByIdUseCase {
    Task<MenuResponse> ExecuteAsync(Guid id, CancellationToken ct = default);
}
