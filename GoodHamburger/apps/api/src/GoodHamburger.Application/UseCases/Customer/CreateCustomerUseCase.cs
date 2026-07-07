using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class CreateCustomerUseCase : ICreateCustomerUseCase {

    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerUseCase> _logger;

    public CreateCustomerUseCase(ICustomerRepository customerRepo, IUnitOfWork unitOfWork,
        ILogger<CreateCustomerUseCase> logger) {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CustomerResponse> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct = default) {

        var phone = Phone.Create(request.Phone);

        var phoneInUse = await _customerRepo.AnyAsync(c => c.Phone == phone, ct);
        if (phoneInUse) {
            // Phone is PII: never log it in clear text.
            _logger.LogWarning("Attempt to register a customer with a phone already in use.");
            throw new ResourceAlreadyExists("Customer", phone.Value);
        }

        var customer = request.ToDomain();

        await _customerRepo.AddOneAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Customer created. Id={CustomerId}", customer.Id);

        return customer.ToResponse();
    }
}
