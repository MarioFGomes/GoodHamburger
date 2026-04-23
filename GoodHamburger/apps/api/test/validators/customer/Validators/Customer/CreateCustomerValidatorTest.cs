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

        request.FirstName =string.Empty;

        var result = validator.Validate(request.ToRequest());

        result.IsValid.Should().BeFalse();
    }

    #endregion
}
