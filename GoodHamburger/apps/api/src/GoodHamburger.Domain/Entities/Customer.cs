using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Domain.Entities;
public class Customer : EntityBase {

    public string FirstName { get; private set; } = string.Empty;
    public string? LastName { get; private set; }
    public string? Address { get; private set; }
    public Phone Phone { get; private set; } = null!;
    public Email Email { get; private set; } = null!;

    protected Customer() { }

    public Customer(string? firstName, string? lastName, Email email, Phone phone, string? address) {
        SetName(firstName, lastName);
        SetContact(email, phone);
        Address = address;
    }

    public void Update(string? firstName, string? lastName, Email email, Phone phone, string? address) {
        SetName(firstName, lastName);
        SetContact(email, phone);
        Address = address;
        Touch();
    }

    private void SetName(string? firstName, string? lastName) {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("FirstName is required.");
        FirstName = firstName.Trim();
        LastName = lastName?.Trim();
    }

    private void SetContact(Email email, Phone phone) {
        Email = email ?? throw new DomainException("Email is required.");
        Phone = phone ?? throw new DomainException("Phone is required.");
    }
}
