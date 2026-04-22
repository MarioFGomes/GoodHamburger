using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Repositories;
public interface IOrderRepository : IBaseRepository<Order> {
    Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetAllWithItemsAsync(int page, int pageSize, CancellationToken ct = default);
}
