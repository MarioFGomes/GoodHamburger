using System.Text.RegularExpressions;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.ValueObjects;

/// <summary>
/// Telephone number in a normalized form (digits with optional leading '+').
/// Two customers with the same phone are the same customer for uniqueness purposes.
/// </summary>
public sealed partial record Phone {

    public string Value { get; private set; } = string.Empty;

    private Phone() { }

    private Phone(string value) => Value = value;

    public static Phone Create(string? raw) {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("Phone is required.");

        var normalized = new string(raw.Where(c => char.IsDigit(c) || c == '+').ToArray());

        if (!PhonePattern().IsMatch(normalized))
            throw new DomainException("Phone must contain 9 to 15 digits, optionally prefixed with '+'.");

        return new Phone(normalized);
    }

    [GeneratedRegex(@"^\+?[0-9]{9,15}$")]
    private static partial Regex PhonePattern();

    public override string ToString() => Value;

    public static implicit operator string(Phone phone) => phone.Value;
}
