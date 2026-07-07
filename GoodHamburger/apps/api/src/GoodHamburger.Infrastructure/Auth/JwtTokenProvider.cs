using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoodHamburger.Application.Auth;
using GoodHamburger.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoodHamburger.Infrastructure.Auth;

public class JwtTokenProvider : ITokenProvider {

    private readonly JwtOptions _options;

    public JwtTokenProvider(IOptions<JwtOptions> options) => _options = options.Value;

    public AuthToken CreateToken(User user) {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        // Algorithm is pinned server-side; tokens signed with anything else are rejected.
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
