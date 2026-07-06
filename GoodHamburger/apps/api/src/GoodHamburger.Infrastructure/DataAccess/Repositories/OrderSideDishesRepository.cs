using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAccess.Repositories; 
public class OrderSideDishesRepository: BaseRepository<OrderSideDishes>, IOrderSideDishesRepository {
    public OrderSideDishesRepository(GoodHamburgerContext _context, ILogger<BaseRepository<OrderSideDishes>> logger) : base(_context, logger) { }
}
