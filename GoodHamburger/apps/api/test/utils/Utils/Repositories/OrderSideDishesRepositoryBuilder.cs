using GoodHamburger.Domain.Repositories;
using Moq;

namespace Utils.Repositories;
public class OrderSideDishesRepositoryBuilder {

    private static OrderSideDishesRepositoryBuilder _instance;
    private readonly Mock<IOrderSideDishesRepository> _repo;

    public OrderSideDishesRepositoryBuilder() {
        _repo = new Mock<IOrderSideDishesRepository>();
    }

    public static OrderSideDishesRepositoryBuilder Instance() {
        _instance = new OrderSideDishesRepositoryBuilder();
        return _instance;
    }

    public OrderSideDishesRepositoryBuilder WithAnyMatch(bool any) {
        _repo.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<GoodHamburger.Domain.Entities.OrderSideDishes, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(any);
        return this;
    }

    public IOrderSideDishesRepository Build() {
        return _repo.Object;
    }
}
