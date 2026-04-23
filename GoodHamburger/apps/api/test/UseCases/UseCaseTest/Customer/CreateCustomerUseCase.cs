using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Application.UseCases.Customer;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Customer;
public class CreateCustomerUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Sucess() {
        var request = CustomerBuilder.Create().ToRequest();
        var useCase = BuildUseCase();
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task PhoneAlreadyExists() {
        var request = CustomerBuilder.Create().ToRequest();
        var customerRepository = CustomerRepositoryBuilder.Instance().WithPhoneExists(true).Build();
        var unitOfWork = UnitOfWorkBuilder.Instance().Build();
        var useCase = new CreateCustomerUseCase(customerRepository, unitOfWork, NullLogger<CreateCustomerUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion

    private CreateCustomerUseCase BuildUseCase() {
        return new CreateCustomerUseCase(
            CustomerRepositoryBuilder.Instance().Build(),
            UnitOfWorkBuilder.Instance().Build(),
            NullLogger<CreateCustomerUseCase>.Instance);
    }
}
