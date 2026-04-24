using GoodHamburger.Domain.Repositories;
using Moq;

namespace Utils.Repositories; 
public class UnitOfWorkBuilder {

    private static UnitOfWorkBuilder _instance;
    private readonly Mock<IUnitOfWork> _repo;

    public UnitOfWorkBuilder()
    {
        if(_repo==null) _repo = new Mock<IUnitOfWork>();
    }

    public static UnitOfWorkBuilder Instance() {
        _instance = new UnitOfWorkBuilder();
        return _instance;
    }

    public IUnitOfWork Build() {
        return _repo.Object;
    }
}
