using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Auth;
public interface IRegisterUserUseCase {
    Task<AuthResponse> ExecuteAsync(RegisterUserRequest request, CancellationToken ct = default);
}
