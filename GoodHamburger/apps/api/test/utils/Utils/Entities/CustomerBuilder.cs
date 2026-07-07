using Bogus;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace Utils.Entities;
public class CustomerBuilder {

    public static Customer Create() {

        return new Faker<Customer>("pt_BR")
              .CustomInstantiator(f => new Customer(
                firstName: f.Person.FirstName,
                lastName: f.Person.LastName,
                email: Email.Create(f.Internet.Email()),
                phone: Phone.Create(f.Phone.PhoneNumber("###########")),
                address: f.Address.FullAddress()))
            .Generate();

    }

    public static List<Customer> CreateMany(int count) {
        return new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                firstName: f.Person.FirstName,
                lastName: f.Person.LastName,
                email: Email.Create(f.Internet.Email()),
                phone: Phone.Create(f.Phone.PhoneNumber("###########")),
                address: f.Address.FullAddress()))
            .Generate(count);
    }
}
