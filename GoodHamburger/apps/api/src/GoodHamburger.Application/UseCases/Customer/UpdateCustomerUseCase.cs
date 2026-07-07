using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class UpdateCustomerUseCase : IUpdateCustomerUseCase {

    private readonly ICustomerRepository _customerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCustomerUseCase> _logger;

    public UpdateCustomerUseCase(ICustomerRepository customerRepo, IUnitOfWork unitOfWork,
        ILogger<UpdateCustomerUseCase> logger) {
        _customerRepo = customerRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CustomerResponse> ExecuteAsync(UpdateCustomerRequest request, CancellationToken ct = default) {

        var customer = await _customerRepo.GetOneAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException("Customer", request.Id);

        var phone = Phone.Create(request.Phone);

        if (customer.Phone != phone) {
            var inUse = await _customerRepo.AnyAsync(
                c => c.Phone == phone && c.Id != request.Id, ct);
            if (inUse)
                throw new ResourceAlreadyExists("Customer", phone.Value);
        }

        // The tracked entity is mutated through the domain method, so identity
        // and CreatedAt are never touched and EF persists a real UPDATE.
        customer.Update(request.FirstName, request.LastName,
            Email.Create(request.Email), phone, request.Address);

        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Customer updated. Id={CustomerId}", customer.Id);

        return customer.ToResponse();
    }
}
