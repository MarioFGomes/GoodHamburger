using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Order;
using GoodHamburger.Domain.Enum;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Order;
public class ConfirmOrderUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var order = OrderBuilder.Create();
        var repo = OrderRepositoryBuilder.Instance().WithOrder(order).Build();
        var useCase = new ConfirmOrderUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<ConfirmOrderUseCase>.Instance);
        var result = await useCase.ExecuteAsync(order.Id);
        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.CONFIRMED);
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = OrderRepositoryBuilder.Instance().WithOrder(null).Build();
        var useCase = new ConfirmOrderUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<ConfirmOrderUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
