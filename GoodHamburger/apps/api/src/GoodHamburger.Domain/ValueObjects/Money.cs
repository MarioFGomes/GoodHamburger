using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.ValueObjects;

/// <summary>
/// Monetary amount bound to a currency. Amounts are rounded to 2 decimal
/// places and can never be negative. Operations across different currencies
/// are rejected instead of silently mixing them.
/// </summary>
public sealed record Money {

    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; } = Currency.BRL;

    private Money() { }

    private Money(decimal amount, Currency currency) {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, Currency currency = Currency.BRL) {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");
        return new Money(decimal.Round(amount, 2, MidpointRounding.ToEven), currency);
    }

    public static Money Zero(Currency currency = Currency.BRL) => new(0m, currency);

    public Money Add(Money other) {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor) {
        if (factor < 0) throw new DomainException("Factor cannot be negative.");
        return new Money(decimal.Round(Amount * factor, 2, MidpointRounding.ToEven), Currency);
    }

    private void EnsureSameCurrency(Money other) {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on different currencies ({Currency} and {other.Currency}).");
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
