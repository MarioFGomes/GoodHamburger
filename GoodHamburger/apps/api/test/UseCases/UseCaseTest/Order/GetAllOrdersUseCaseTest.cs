using FluentAssertions;
using GoodHamburger.Application.UseCases.Order;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Order;
public class GetAllOrdersUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var orders = new List<GoodHamburger.Domain.Entities.Order> { OrderBuilder.Create() };
        var repo = OrderRepositoryBuilder.Instance()
            .WithOrders(orders)
            .WithCount(orders.Count)
            .Build();
        var useCase = new GetAllOrdersUseCase(repo);
        var result = await useCase.ExecuteAsync(1, 10);
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task InvalidPageNormalized() {
        var repo = OrderRepositoryBuilder.Instance()
            .WithOrders(new List<GoodHamburger.Domain.Entities.Order>())
            .WithCount(0)
            .Build();
        var useCase = new GetAllOrdersUseCase(repo);
        var result = await useCase.ExecuteAsync(0, 0);
        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion
}
