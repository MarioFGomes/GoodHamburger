using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Domain.Entities;
public class User : EntityBase {

    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.USER;
    public DateTime? LastLoginAt { get; private set; }

    protected User() { }

    public User(string? name, Email email, string passwordHash, UserRole role) {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");
        Name = name.Trim();
        Email = email ?? throw new DomainException("Email is required.");
        SetPasswordHash(passwordHash);
        Role = role;
    }

    public void SetPasswordHash(string passwordHash) {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");
        PasswordHash = passwordHash;
        Touch();
    }

    public void RegisterLogin() {
        LastLoginAt = DateTime.UtcNow;
        Touch();
    }
}
