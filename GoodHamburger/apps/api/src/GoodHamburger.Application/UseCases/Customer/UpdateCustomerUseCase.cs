using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class UpdateCustomerUseCase : IUpdateCustomerUseCase {

    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCustomerUseCase> _logger;

    public UpdateCustomerUseCase(ICustomerRepository customerRepo, IUnitOfWork unitOfWork,
        ILogger<UpdateCustomerUseCase> logger)
    {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken ct = default) {

        var customer = await _customerRepo.GetOneAsync(i=>i.Id==request.Id, ct)
            ?? throw new NotFoundException("Customer", request.Id);

      
        if (customer.Phone != request.Phone) {
            var inUse = await _customerRepo.AnyAsync(
                c => c.Phone == request.Phone && c.Id != request.Id, ct);
            if (inUse)
                throw new ResourceAlreadyExists("Customer", request.Phone);
        }

        var customerToUpdate = request.ToDomain();

        customerToUpdate.Id=customer.Id;
        
        await _customerRepo.ReplaceOneAsync(i=>i.Id==customer.Id, customerToUpdate, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Customer updated. Id={CustomerId}", customer.Id);

        return customerToUpdate.ToResponse();
    }
}
