using FluentAssertions;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Order;
using GoodHamburger.Domain.Enum;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Order;
public class CreateOrderUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var menu = MenuBuilder.Create().ToEntity();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = menu.Id,
            SideDishIds = new List<Guid>()
        };
        var useCase = BuildUseCase(customerExists: true, menu: menu);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
        result.Discount.Should().Be(0);
    }

    [Fact]
    public async Task SuccessWithFries() {
        var menu = MenuBuilder.Create().ToEntity();
        var fries = SideDishesBuilder.CreateFries().ToEntity();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { fries.Id }
        };
        var sideDishRepo = SideDishesRepositoryBuilder.Instance().WithSideDish(fries).Build();
        var useCase = BuildUseCase(customerExists: true, menu: menu, sideDishRepo: sideDishRepo);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
        result.Discount.Should().Be(10);
    }

    [Fact]
    public async Task SuccessWithDrink() {
        var menu = MenuBuilder.Create().ToEntity();
        var drink = SideDishesBuilder.CreateDrink().ToEntity();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { drink.Id }
        };
        var sideDishRepo = SideDishesRepositoryBuilder.Instance().WithSideDish(drink).Build();
        var useCase = BuildUseCase(customerExists: true, menu: menu, sideDishRepo: sideDishRepo);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
        result.Discount.Should().Be(15);
    }

    [Fact]
    public async Task SuccessWithCombo() {
        var menu = MenuBuilder.Create().ToEntity();
        var fries = SideDishesBuilder.CreateFries().ToEntity();
        var drink = SideDishesBuilder.CreateDrink().ToEntity();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { fries.Id, drink.Id }
        };

        var sideDishRepoMock = new Mock<GoodHamburger.Domain.Repositories.ISideDishesRepository>();
        sideDishRepoMock
            .SetupSequence(r => r.GetOneAsync(It.IsAny<System.Linq.Expressions.Expression<Func<GoodHamburger.Domain.Entities.SideDishes, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fries)
            .ReturnsAsync(drink);

        var useCase = BuildUseCase(customerExists: true, menu: menu, sideDishRepo: sideDishRepoMock.Object);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
        result.Discount.Should().Be(20);
    }

    #endregion

    #region Fail

    [Fact]
    public async Task CustomerNotFound() {
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid>()
        };
        var useCase = BuildUseCase(customerExists: false);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task MenuNotFound() {
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = Guid.NewGuid(),
            SideDishIds = new List<Guid>()
        };
        var useCase = BuildUseCase(customerExists: true, menu: null);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SideDishNotFound() {
        var menu = MenuBuilder.Create().ToEntity();
        var request = new CreateOrderRequest {
            CustomerId = Guid.NewGuid(),
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { Guid.NewGuid() }
        };
        var sideDishRepo = SideDishesRepositoryBuilder.Instance().WithSideDish(null).Build();
        var useCase = BuildUseCase(customerExists: true, menu: menu, sideDishRepo: sideDishRepo);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    private CreateOrderUseCase BuildUseCase(
        bool customerExists = true,
        GoodHamburger.Domain.Entities.Menu? menu = null,
        GoodHamburger.Domain.Repositories.ISideDishesRepository? sideDishRepo = null) {

        var customerRepo = CustomerRepositoryBuilder.Instance().WithPhoneExists(customerExists).Build();
        var menuRepo = MenuRepositoryBuilder.Instance().WithMenu(menu).Build();
        var orderRepo = OrderRepositoryBuilder.Instance().WithCount(0).Build();
        sideDishRepo ??= SideDishesRepositoryBuilder.Instance().Build();

        return new CreateOrderUseCase(
            orderRepo,
            customerRepo,
            menuRepo,
            sideDishRepo,
            UnitOfWorkBuilder.Instance().Build(),
            NullLogger<CreateOrderUseCase>.Instance);
    }
}
