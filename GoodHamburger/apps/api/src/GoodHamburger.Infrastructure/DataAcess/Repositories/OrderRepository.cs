using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories;
public class OrderRepository : BaseRepository<Order>, IOrderRepository {
    public OrderRepository(GoodHamburgerContext context, ILogger<BaseRepository<Order>> logger) : base(context, logger) { }

    public async Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default) {
        return await GetQueryable()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderSideDishes)
                    .ThenInclude(s => s.SideDishes)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<Order>> GetAllWithItemsAsync(int page, int pageSize, CancellationToken ct = default) {
        return await GetQueryable()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderSideDishes)
                    .ThenInclude(s => s.SideDishes)
            .OrderBy(o => o.OrderNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> NextOrderNumberAsync(CancellationToken ct = default) {
        var max = await GetQueryable().MaxAsync(o => (int?)o.OrderNumber, ct);
        return (max ?? 0) + 1;
    }
}
