using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Customer;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Customer;
public class GetCustomerByIdUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var customer = CustomerBuilder.Create();
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(customer).Build();
        var useCase = new GetCustomerByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(customer.Id);
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(null).Build();
        var useCase = new GetCustomerByIdUseCase(repo);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
