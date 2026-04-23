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

    public IOrderItemRepository Build() {
        return _repo.Object;
    }
}
