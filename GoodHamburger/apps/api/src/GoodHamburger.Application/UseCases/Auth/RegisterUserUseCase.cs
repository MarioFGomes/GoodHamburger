using GoodHamburger.Application.Auth;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Auth;
public class RegisterUserUseCase : IRegisterUserUseCase {

    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserUseCase> _logger;

    public RegisterUserUseCase(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserUseCase> logger) {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterUserRequest request, CancellationToken ct = default) {

        var email = Email.Create(request.Email);

        var emailInUse = await _userRepo.AnyAsync(u => u.Email == email, ct);
        if (emailInUse)
            throw new ResourceAlreadyExists("User", email.Value);

        // Self-service registration always produces a regular user.
        // Admins are provisioned via configuration seeding only.
        var user = new User(request.Name, email, _passwordHasher.Hash(request.Password!), UserRole.USER);

        await _userRepo.AddOneAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("User registered. Id={UserId}, Role={Role}", user.Id, user.Role);

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
