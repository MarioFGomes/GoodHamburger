using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class CreateCustomerUseCase : ICreateCustomerUseCase {

    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerRequest> _logger;

    public CreateCustomerUseCase(ICustomerRepository customerRepo, IUnitOfWork unitOfWork,
        ILogger<CreateCustomerRequest> logger)
    {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<CustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct = default) {
        var CustomerAlreadyExists = await _customerRepo.AnyAsync(i => i.Phone.Equals(request.Phone),ct);
        if (CustomerAlreadyExists)   throw new ResourceAlreadyExists("Customer", request.FirstName);
        await _customerRepo.AddOneAsync(request.ToDomain(),ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Customer created");
        return request.ToDomain().ToResponse();
    }
}
