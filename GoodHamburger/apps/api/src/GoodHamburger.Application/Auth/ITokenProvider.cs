using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Auth;

public record AuthToken(string AccessToken, DateTime ExpiresAtUtc);

public interface ITokenProvider {
    AuthToken CreateToken(User user);
}
