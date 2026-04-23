using FluentAssertions;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.Validators;
using Utils.Entities;

namespace Validators.Customer;
public class UpdateCustomerValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new UpdateCustomerRequestValidator();
        var customer = CustomerBuilder.Create();
        var request = BuildValidRequest(customer);
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateFirstNameEmpty() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.FirstName = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLastNameEmpty() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.LastName = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePhoneEmpty() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.Phone = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePhoneInvalidFormat() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.Phone = "abc";
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateEmailEmpty() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.Email = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateEmailInvalidFormat() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.Email = "nao-e-email";
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateFirstNameExceedsMaxLength() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.FirstName = new string('a', 101);
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLastNameExceedsMaxLength() {
        var validator = new UpdateCustomerRequestValidator();
        var request = BuildValidRequest(CustomerBuilder.Create());
        request.LastName = new string('a', 101);
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    #endregion

    private static UpdateCustomerRequest BuildValidRequest(GoodHamburger.Domain.Entities.Customer customer) {
        return new UpdateCustomerRequest {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address
        };
    }
}
