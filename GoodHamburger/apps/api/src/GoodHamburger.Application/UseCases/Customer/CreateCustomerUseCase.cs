using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class CreateCustomerUseCase : ICreateCustomerUseCase {

    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerUseCase> _logger;

    public CreateCustomerUseCase(ICustomerRepository customerRepo, IUnitOfWork unitOfWork,
        ILogger<CreateCustomerUseCase> logger)
    {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<CustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct = default) {

        var phoneInUse = await _customerRepo.AnyAsync(i => i.Phone == request.Phone, ct);
        
            if (phoneInUse) {
            
            _logger.LogWarning(
                "Attempt to register an already existing customer.. Phone={Phone}",
                request.Phone);

            throw new ResourceAlreadyExists("Customer", request.Phone);
        }
        
            var customer = request.ToDomain();

            await _customerRepo.AddOneAsync(customer, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                  "Customer created. Id={CustomerId}, Name={FirstName}",
                  customer.Id,
                  customer.FirstName);

            return customer.ToResponse();
        
    }
}
