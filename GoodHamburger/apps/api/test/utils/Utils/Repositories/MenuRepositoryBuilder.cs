using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Moq;
using System.Linq.Expressions;

namespace Utils.Repositories;
public class MenuRepositoryBuilder {

    private static MenuRepositoryBuilder _instance;
    private readonly Mock<IMenuRepository> _repo;

    public MenuRepositoryBuilder() {
        _repo = new Mock<IMenuRepository>();
    }

    public static MenuRepositoryBuilder Instance() {
        _instance = new MenuRepositoryBuilder();
        return _instance;
    }

    public MenuRepositoryBuilder WithNameExists(bool exists = true) {
        _repo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Menu, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(exists);
        return this;
    }

    public MenuRepositoryBuilder WithMenu(Menu? menu) {
        _repo.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Menu, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(menu);
        return this;
    }

    public MenuRepositoryBuilder WithMenus(IEnumerable<Menu> list) {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
             .ReturnsAsync(list);
        return this;
    }

    public MenuRepositoryBuilder WithCount(int count) {
        _repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(count);
        return this;
    }

    public IMenuRepository Build() {
        return _repo.Object;
    }
}
