using FluentAssertions;
using GoodHamburger.Application.Validators;
using GoodHamburger.Domain.Enum;
using Utils.Entities;

namespace Validators.Menu;
public class UpdateMenuValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateNameEmpty() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateNameExceedsMaxLength() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Name = new string('a', 101);
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceNull() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Price = null;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceMustBeGreaterThanZero() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Price = 0;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateCurrencyInvalidValue() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Currency = (Currency)99;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateStatusInvalidValue() {
        var validator = new UpdateMenuRequestValidator();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Status = (MenuStatus)99;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
