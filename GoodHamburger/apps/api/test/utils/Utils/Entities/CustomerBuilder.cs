using Bogus;
using Bogus.DataSets;
using GoodHamburger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Entities; 
public class CustomerBuilder {

    public static Customer Create() {
        
        return  new Faker<Customer>("pt_BR")
              .CustomInstantiator(f => new Customer(
                firtsname: f.Person.FirstName,
                lastName: f.Person.LastName,
                email: f.Internet.Email(),
                phone: f.Phone.PhoneNumber("###########"),
                address: f.Address.FullAddress()))
            .Generate();

    }

    public static List<Customer> CreateMany(int count) {
        return new Faker<Customer>()
            .CustomInstantiator(f => new Customer(
                firtsname: f.Person.FirstName,
                lastName: f.Person.LastName,
                email: f.Internet.Email(),
                phone: f.Phone.PhoneNumber("###########"),
                address: f.Address.FullAddress()))
            .Generate(count);
    }
}
