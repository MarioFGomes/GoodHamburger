using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Order;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Order;
public class GetOrderByIdUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var order = OrderBuilder.Create();
        var repo = OrderRepositoryBuilder.Instance().WithOrder(order).Build();
        var useCase = new GetOrderByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(order.Id);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = OrderRepositoryBuilder.Instance().WithOrder(null).Build();
        var useCase = new GetOrderByIdUseCase(repo);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
