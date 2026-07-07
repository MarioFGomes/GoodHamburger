using Asp.Versioning;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[AllowAnonymous]
[EnableRateLimiting(ApiBootstrapper.AuthRateLimitPolicy)]
public class AuthController : EntityController {

    private readonly IRegisterUserUseCase _register;
    private readonly ILoginUseCase _login;

    public AuthController(IRegisterUserUseCase register, ILoginUseCase login) {
        _register = register;
        _login = login;
    }

    /// <summary>Creates a regular (USER) account and returns an access token.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct) {
        var response = await _register.ExecuteAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponse>.Ok(response, "User registered.", StatusCodes.Status201Created));
    }

    /// <summary>Exchanges e-mail + password for a JWT access token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct) {
        var response = await _login.ExecuteAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Authenticated."));
    }
}
