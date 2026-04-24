using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Moq;
using System.Linq.Expressions;

namespace Utils.Repositories;
public class CustomerRepositoryBuilder {

    private static CustomerRepositoryBuilder _instance;
    private readonly Mock<ICustomerRepository> _repo;

    public CustomerRepositoryBuilder() {
        _repo = new Mock<ICustomerRepository>();
    }

    public static CustomerRepositoryBuilder Instance() {
        _instance = new CustomerRepositoryBuilder();
        return _instance;
    }

    public CustomerRepositoryBuilder WithPhoneExists(bool exists = true) {
        _repo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(exists);
        return this;
    }

    public CustomerRepositoryBuilder WithCustomer(Customer? customer) {
        _repo.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(customer);
        return this;
    }

    public CustomerRepositoryBuilder WithCustomers(IEnumerable<Customer> list) {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>()))
             .ReturnsAsync(list);
        return this;
    }

    public CustomerRepositoryBuilder WithCount(int count) {
        _repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(count);
        return this;
    }

    public ICustomerRepository Build() {
        return _repo.Object;
    }
}
