using GoodHamburger.Application.Auth;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Auth;
public class LoginUseCase : ILoginUseCase {

    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        ILogger<LoginUseCase> logger) {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default) {

        Email email;
        try {
            email = Email.Create(request.Email);
        } catch (DomainException) {
            // A malformed e-mail must look exactly like a wrong one to the caller.
            throw new InvalidCredentialsException();
        }

        var user = await _userRepo.GetOneAsync(u => u.Email == email, ct);

        if (user is null || !_passwordHasher.Verify(request.Password ?? string.Empty, user.PasswordHash)) {
            _logger.LogWarning("Failed login attempt.");
            throw new InvalidCredentialsException();
        }

        user.RegisterLogin();
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("User logged in. Id={UserId}", user.Id);

        var token = _tokenProvider.CreateToken(user);
        return new AuthResponse {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            Email = user.Email.Value,
            Name = user.Name,
            Role = user.Role.ToString(),
        };
    }
}
