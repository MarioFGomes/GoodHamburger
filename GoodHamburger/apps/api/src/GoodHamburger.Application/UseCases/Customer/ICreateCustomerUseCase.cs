using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Customer;
public interface ICreateCustomerUseCase
{
    Task<CustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct = default);
}
