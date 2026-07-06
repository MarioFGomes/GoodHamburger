using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Moq;

namespace Utils.Repositories;
public class OrderItemRepositoryBuilder {

    private static OrderItemRepositoryBuilder _instance;
    private readonly Mock<IOrderItemRepository> _repo;

    public OrderItemRepositoryBuilder() {
        _repo = new Mock<IOrderItemRepository>();
    }

    public static OrderItemRepositoryBuilder Instance() {
        _instance = new OrderItemRepositoryBuilder();
        return _instance;
    }

    public OrderItemRepositoryBuilder WithAnyMatch(bool any) {
        _repo.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderItem, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(any);
        return this;
    }

    public IOrderItemRepository Build() {
        return _repo.Object;
    }
}
