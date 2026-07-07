using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAccess.Repositories;
public class OrderRepository : BaseRepository<Order>, IOrderRepository {

    private readonly GoodHamburgerContext _context;

    public OrderRepository(GoodHamburgerContext context, ILogger<BaseRepository<Order>> logger) : base(context, logger) {
        _context = context;
    }

    private IQueryable<Order> QueryWithItems() =>
        GetQueryable()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.OrderSideDishes)
                    .ThenInclude(s => s.SideDishes);

    public async Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default) {
        return await QueryWithItems().FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<Order?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct = default) {
        return await QueryWithItems().FirstOrDefaultAsync(o => o.IdempotencyKey == idempotencyKey, ct);
    }

    public async Task<IEnumerable<Order>> GetAllWithItemsAsync(int page, int pageSize, CancellationToken ct = default) {
        return await QueryWithItems()
            .OrderBy(o => o.OrderNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> NextOrderNumberAsync(CancellationToken ct = default) {
        // The database sequence is atomic, so concurrent orders can never draw
        // the same number. MAX+1 stays only for the InMemory provider (tests),
        // which does not support sequences.
        if (_context.Database.IsSqlServer()) {
            return await _context.Database
                .SqlQuery<int>($"SELECT NEXT VALUE FOR [OrderNumbers] AS [Value]")
                .SingleAsync(ct);
        }

        var max = await GetQueryable().IgnoreQueryFilters().MaxAsync(o => (int?)o.OrderNumber, ct);
        return (max ?? 0) + 1;
    }
}
