using FluentAssertions;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Customer;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Customer;
public class UpdateCustomerUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var existingCustomer = CustomerBuilder.Create();
        var request = new UpdateCustomerRequest {
            Id = existingCustomer.Id,
            FirstName = "NovoNome",
            LastName = existingCustomer.LastName,
            Phone = existingCustomer.Phone,
            Email = existingCustomer.Email,
            Address = existingCustomer.Address
        };
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(existingCustomer).Build();
        var useCase = new UpdateCustomerUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateCustomerUseCase>.Instance);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task CustomerNotFound() {
        var request = new UpdateCustomerRequest {
            Id = Guid.NewGuid(),
            FirstName = "Nome",
            LastName = "Sobrenome",
            Phone = "123456789",
            Email = "test@test.com",
            Address = "Rua A"
        };
        var repo = CustomerRepositoryBuilder.Instance().WithCustomer(null).Build();
        var useCase = new UpdateCustomerUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateCustomerUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task PhoneAlreadyExists() {
        var existingCustomer = CustomerBuilder.Create();
        var request = new UpdateCustomerRequest {
            Id = existingCustomer.Id,
            FirstName = existingCustomer.FirstName,
            LastName = existingCustomer.LastName,
            Phone = "999999999",
            Email = existingCustomer.Email,
            Address = existingCustomer.Address
        };
        var repo = CustomerRepositoryBuilder.Instance()
            .WithCustomer(existingCustomer)
            .WithPhoneExists(true)
            .Build();
        var useCase = new UpdateCustomerUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateCustomerUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion
}
