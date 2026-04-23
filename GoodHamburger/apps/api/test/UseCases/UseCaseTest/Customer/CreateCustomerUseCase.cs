using GoodHamburger.Application.Mappers;
using GoodHamburger.Application.UseCases.Customer;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;
using FluentAssertions;

namespace UseCaseTest.Customer; 
public class CreateCustomerUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Sucess() {
        var request = CustomerBuilder.Create().ToRequest();
        var UseCase = CreateCustomerUseCaseBuilder();
        var result = await UseCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }
 #endregion


    private CreateCustomerUseCase CreateCustomerUseCaseBuilder() 
    {
        var logger = NullLogger<CreateCustomerUseCase>.Instance;

        var CustomerRepository = CustomerRepositoryBuilder.Instance().Build();

        var unitOfWork = UnitOfWorkBuilder.Instance().Build();

        return new CreateCustomerUseCase(CustomerRepository, unitOfWork, logger);
    }
}
