using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Order;
using GoodHamburger.Application.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Order;
public class DeleteOrderUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var order = OrderBuilder.Create();
        var repo = OrderRepositoryBuilder.Instance().WithOrder(order).Build();
        var useCase = BuildUseCase(repo);
        var act = () => useCase.ExecuteAsync(order.Id);
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = OrderRepositoryBuilder.Instance().WithOrder(null).Build();
        var useCase = BuildUseCase(repo);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task InvalidStatusCannotDelete() {
        var order = OrderBuilder.Create();
        order.Confirm();
        var repo = OrderRepositoryBuilder.Instance().WithOrder(order).Build();
        var useCase = BuildUseCase(repo);
        var act = () => useCase.ExecuteAsync(order.Id);
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    #endregion

    private DeleteOrderUseCase BuildUseCase(GoodHamburger.Domain.Repositories.IOrderRepository orderRepo) {
        return new DeleteOrderUseCase(
            orderRepo,
            OrderItemRepositoryBuilder.Instance().Build(),
            OrderSideDishesRepositoryBuilder.Instance().Build(),
            UnitOfWorkBuilder.Instance().Build(),
            NullLogger<DeleteOrderUseCase>.Instance);
    }
}
