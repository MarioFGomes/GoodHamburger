using FluentAssertions;
using GoodHamburger.Application.Validators;
using GoodHamburger.Domain.Enum;
using Utils.Entities;

namespace Validators.Menu;
public class CreateMenuValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateNameEmpty() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateNameExceedsMaxLength() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        request.Name = new string('a', 101);
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceNull() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        request.Price = null;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceMustBeGreaterThanZero() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        request.Price = 0;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateCurrencyInvalidValue() {
        var validator = new CreateMenuRequestValidator();
        var request = MenuBuilder.Create().ToRequest();
        request.Currency = (Currency)99;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
