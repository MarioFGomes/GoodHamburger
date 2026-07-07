using FluentAssertions;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.ValueObjects;

namespace DomainTest.ValueObjects;

public class PhoneTest {

    [Theory]
    [InlineData("+5521975342254", "+5521975342254")]
    [InlineData("+55 21 97534-2254", "+5521975342254")]
    [InlineData("(21) 97534-2254", "21975342254")]
    public void Create_NormalizesFormatting(string raw, string expected) {
        Phone.Create(raw).Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("12345678901234567890")]
    public void Create_InvalidInput_Throws(string? raw) {
        var act = () => Phone.Create(raw);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_IsByValue() {
        Phone.Create("+55 21 97534-2254").Should().Be(Phone.Create("+5521975342254"));
    }
}

public class EmailTest {

    [Fact]
    public void Create_NormalizesToLowerCase() {
        Email.Create("  Maria.Souza@Test.COM ").Value.Should().Be("maria.souza@test.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("a@b")]
    [InlineData("two@@test.com")]
    public void Create_InvalidInput_Throws(string? raw) {
        var act = () => Email.Create(raw);
        act.Should().Throw<DomainException>();
    }
}

public class MoneyTest {

    [Fact]
    public void Create_RoundsToTwoDecimals() {
        Money.Create(10.005m).Amount.Should().Be(10.00m);
        Money.Create(10.015m).Amount.Should().Be(10.02m);
    }

    [Fact]
    public void Create_NegativeAmount_Throws() {
        var act = () => Money.Create(-1m);
        act.Should().Throw<DomainException>().WithMessage("*negative*");
    }

    [Fact]
    public void Add_SameCurrency_Sums() {
        Money.Create(5m).Add(Money.Create(2.5m)).Should().Be(Money.Create(7.5m));
    }

    [Fact]
    public void Add_DifferentCurrency_Throws() {
        var act = () => Money.Create(5m, Currency.BRL).Add(Money.Create(1m, Currency.USD));
        act.Should().Throw<DomainException>().WithMessage("*currencies*");
    }

    [Fact]
    public void Equality_IsByValue() {
        Money.Create(9.90m).Should().Be(Money.Create(9.90m));
        Money.Create(9.90m, Currency.BRL).Should().NotBe(Money.Create(9.90m, Currency.USD));
    }
}
