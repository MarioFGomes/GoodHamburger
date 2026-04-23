using GoodHamburger.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Repositories; 
public class CustomerRepositoryBuilder {
   
    private static CustomerRepositoryBuilder _instance;
    private readonly Mock<ICustomerRepository> _repo;

    public CustomerRepositoryBuilder() {
        if (_repo == null) _repo = new Mock<ICustomerRepository>();
    }

    public static CustomerRepositoryBuilder Instance() {
        _instance = new CustomerRepositoryBuilder();
        return _instance;
    }

    public ICustomerRepository Build() {
        return _repo.Object;
    }
}
