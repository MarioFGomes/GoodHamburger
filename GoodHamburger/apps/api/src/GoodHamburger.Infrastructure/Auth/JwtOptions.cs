namespace GoodHamburger.Infrastructure.Auth;

public class JwtOptions {
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "GoodHamburger.API";
    public string Audience { get; set; } = "GoodHamburger.Clients";
    public int ExpiryMinutes { get; set; } = 60;
}
