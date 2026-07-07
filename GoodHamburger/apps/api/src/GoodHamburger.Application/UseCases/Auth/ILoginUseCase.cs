using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Auth;
public interface ILoginUseCase {
    Task<AuthResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default);
}
