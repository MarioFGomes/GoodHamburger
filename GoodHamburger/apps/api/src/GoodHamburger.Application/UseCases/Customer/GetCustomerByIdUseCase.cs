using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;


namespace GoodHamburger.Application.UseCases.Customer;
public class GetCustomerByIdUseCase : IGetCustomerByIdUseCase {

    private readonly ICustomerRepository _customerRepo;

    public GetCustomerByIdUseCase(ICustomerRepository repo) => _customerRepo = repo;

    public async Task<CustomerResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var customer = await _customerRepo.GetOneAsync(i => i.Id == id,ct);
        if (customer is null) throw new NotFoundException("Customer",id);
        return customer.ToResponse();
    }
}
