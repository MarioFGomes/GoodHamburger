using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class OrderRepository: BaseRepository<Order>, IOrderRepository {
    public OrderRepository(GoodHamburgerContext _context, ILogger<BaseRepository<Order>> logger) : base(_context, logger) { }
}
