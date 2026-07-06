using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.Customer;
public class DeleteCustomerUseCase : IDeleteCustomerUseCase {

        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCustomerUseCase> _logger;

        public DeleteCustomerUseCase(
            ICustomerRepository customerRepo,
            IOrderRepository orderRepo,
            IUnitOfWork unitOfWork,
            ILogger<DeleteCustomerUseCase> logger) {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken ct = default) {
            var customer = await _customerRepo.GetOneAsync(i=>i.Id==id, ct)
                ?? throw new NotFoundException("Customer", id);

            var hasOrders = await _orderRepo.AnyAsync(o => o.CustomerID == id, ct);
            if (hasOrders)
                throw new BusinessRuleException("Customer has orders and cannot be deleted.");

            await _customerRepo.DeleteAsync(i=>i.Id==customer.Id,ct);
           
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Customer deleted. Id={CustomerId}", id);
        }
    }

