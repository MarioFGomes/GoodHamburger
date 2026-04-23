using FluentAssertions;
using GoodHamburger.Application.UseCases.Customer;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Customer;
public class GetAllCustomersUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var customers = CustomerBuilder.CreateMany(3);
        var repo = CustomerRepositoryBuilder.Instance()
            .WithCustomers(customers)
            .WithCount(customers.Count)
            .Build();
        var useCase = new GetAllCustomersUseCase(repo);
        var result = await useCase.ExecuteAsync(1, 10);
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task InvalidPageNormalized() {
        var repo = CustomerRepositoryBuilder.Instance()
            .WithCustomers(new List<GoodHamburger.Domain.Entities.Customer>())
            .WithCount(0)
            .Build();
        var useCase = new GetAllCustomersUseCase(repo);
        var result = await useCase.ExecuteAsync(0, 0);
        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion
}
