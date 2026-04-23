using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Customer;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Customer;
public class DeleteCustomerUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var customer = CustomerBuilder.Create();
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(customer).Build();
        var useCase = new DeleteCustomerUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteCustomerUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(customer.Id);
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task CustomerNotFound() {
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(null).Build();
        var useCase = new DeleteCustomerUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteCustomerUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
