using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;

namespace GoodHamburger.Application.UseCases.Customer; 
public interface IUpdateCustomerUseCase {
    Task<CustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken ct = default);
}
