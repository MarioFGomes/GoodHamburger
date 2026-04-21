using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class CustomerRepository: BaseRepository<Customer>, ICustomerRepository {
    public CustomerRepository(GoodHamburgerContext _context, ILogger<BaseRepository<Customer>> logger) : base(_context, logger) { }
}
