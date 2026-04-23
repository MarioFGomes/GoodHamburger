using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Moq;
using System.Linq.Expressions;

namespace Utils.Repositories;
public class SideDishesRepositoryBuilder {

    private static SideDishesRepositoryBuilder _instance;
    private readonly Mock<ISideDishesRepository> _repo;

    public SideDishesRepositoryBuilder() {
        _repo = new Mock<ISideDishesRepository>();
    }

    public static SideDishesRepositoryBuilder Instance() {
        _instance = new SideDishesRepositoryBuilder();
        return _instance;
    }

    public SideDishesRepositoryBuilder WithNameExists(bool exists = true) {
        _repo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<SideDishes, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(exists);
        return this;
    }

    public SideDishesRepositoryBuilder WithSideDish(SideDishes? sideDish) {
        _repo.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<SideDishes, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(sideDish);
        return this;
    }

    public SideDishesRepositoryBuilder WithSideDishes(IEnumerable<SideDishes> list) {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
             .ReturnsAsync(list);
        return this;
    }

    public SideDishesRepositoryBuilder WithCount(int count) {
        _repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(count);
        return this;
    }

    public ISideDishesRepository Build() {
        return _repo.Object;
    }
}
