using System.Text.RegularExpressions;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.ValueObjects;

/// <summary>
/// E-mail address normalized to lower case. Validation is intentionally
/// pragmatic (RFC-complete e-mail regexes reject real addresses).
/// </summary>
public sealed partial record Email {

    public string Value { get; private set; } = string.Empty;

    private Email() { }

    private Email(string value) => Value = value;

    public static Email Create(string? raw) {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("Email is required.");

        var normalized = raw.Trim().ToLowerInvariant();

        if (normalized.Length > 200 || !EmailPattern().IsMatch(normalized))
            throw new DomainException("Email format is invalid.");

        return new Email(normalized);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
