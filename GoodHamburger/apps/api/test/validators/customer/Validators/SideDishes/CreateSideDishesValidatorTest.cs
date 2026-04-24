using FluentAssertions;
using GoodHamburger.Application.Validators;
using GoodHamburger.Domain.Enum;
using Utils.Entities;

namespace Validators.SideDishes;
public class CreateSideDishesValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateNameEmpty() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateNameExceedsMaxLength() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Name = new string('a', 101);
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceNull() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Price = null;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePriceMustBeGreaterThanZero() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Price = 0;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateCategoryInvalidValue() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Category = (SideDishCategory)99;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateCurrencyInvalidValue() {
        var validator = new CreateSideDishesRequestValidator();
        var request = SideDishesBuilder.Create().ToRequest();
        request.Currency = (Currency)99;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
