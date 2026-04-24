using FluentAssertions;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.Validators;

namespace Validators.Order;
public class CreateOrderValidatorTest {

    #region success

    [Fact]
    public void ValidateSuccess() {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid>()
        };
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateSuccessWithTwoSideDishes() {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };
        var result = validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Fail

    [Fact]
    public void ValidateCustomerIdEmpty() {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest {
            CustomerId = Guid.Empty,
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid>()
        };
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateMenuIdEmpty() {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.Empty,
            SideDishIds = new List<Guid>()
        };
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateSideDishIdsExceedMaximum() {
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
        };
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
