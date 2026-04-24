using FluentAssertions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Application.Validators;
using Utils.Entities;

namespace Validators.Customer;
public class CreateCustomerValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateFirstNameEmpty() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.FirstName = string.Empty;
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLastNameEmpty() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.LastName = string.Empty;
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePhoneEmpty() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.Phone = string.Empty;
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePhoneInvalidFormat() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.Phone = "abc";
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateEmailEmpty() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.Email = string.Empty;
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateEmailInvalidFormat() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.Email = "nao-e-email";
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateFirstNameExceedsMaxLength() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.FirstName = new string('a', 101);
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLastNameExceedsMaxLength() {
        var validator = new CreateCustomerRequestValidator();
        var request = CustomerBuilder.Create();
        request.LastName = new string('a', 101);
        var result = validator.Validate(request.ToRequest());
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
