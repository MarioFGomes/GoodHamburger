using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Moq;

namespace Utils.Repositories;
public class OrderRepositoryBuilder {

    private static OrderRepositoryBuilder _instance;
    private readonly Mock<IOrderRepository> _repo;

    public OrderRepositoryBuilder() {
        _repo = new Mock<IOrderRepository>();
    }

    public static OrderRepositoryBuilder Instance() {
        _instance = new OrderRepositoryBuilder();
        return _instance;
    }

    public OrderRepositoryBuilder WithOrder(Order? order) {
        _repo.Setup(r => r.GetWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(order);
        return this;
    }

    public OrderRepositoryBuilder WithOrders(IEnumerable<Order> list) {
        _repo.Setup(r => r.GetAllWithItemsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(list);
        return this;
    }

    public OrderRepositoryBuilder WithCount(int count) {
        _repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(count);
        return this;
    }

    public IOrderRepository Build() {
        return _repo.Object;
    }
}
