using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class OrderItemRepository: BaseRepository<OrderItem>, IOrderItemRepository {

    public OrderItemRepository(GoodHamburgerContext _context, ILogger<BaseRepository<OrderItem>> logger) : base(_context, logger) { }
}
