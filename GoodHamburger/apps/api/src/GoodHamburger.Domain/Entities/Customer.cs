using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;
public class Customer: EntityBase {

    private string? _firstName;

    public string? FirstName {
        get => _firstName;
        set {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new DomainException("FirstName não pode ser vazio.");
            _firstName = value;
        }
    }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public Customer(){}
    public Customer(string firstName, string lastName, string email, string phone, string address) {
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        Phone = phone;
        Email = email;
    }

}
