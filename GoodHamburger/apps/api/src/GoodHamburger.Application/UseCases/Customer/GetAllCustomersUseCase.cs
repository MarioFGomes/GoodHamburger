using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.Customer;
public class GetAllCustomersUseCase : IGetAllCustomersUseCase {
    
    private readonly ICustomerRepository _customerRepo;

    public GetAllCustomersUseCase(ICustomerRepository _repo) => _customerRepo = _repo;
    public async Task<PagedResponse<CustomerResponse>> ExecuteAsync(int page, int pageSize, CancellationToken ct = default) {

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var customers = await _customerRepo.GetAllAsync(ct,page, pageSize);
        
        var total = await _customerRepo.CountAsync(ct);

        return new PagedResponse<CustomerResponse> {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Items = customers.Select(c => c.ToResponse()).ToList()
        };
    }
}
