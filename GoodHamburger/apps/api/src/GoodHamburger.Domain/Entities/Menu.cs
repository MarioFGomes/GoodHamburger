using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Domain.Entities;
public class Menu : EntityBase {

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = null!;
    public MenuStatus Status { get; private set; } = MenuStatus.Available;

    protected Menu() { }

    public Menu(string? name, string? description, Money price) {
        Rename(name);
        Description = description;
        ChangePrice(price);
    }

    public void Update(string? name, string? description, Money price, MenuStatus status) {
        Rename(name);
        Description = description;
        ChangePrice(price);
        Status = status;
        Touch();
    }

    public void Rename(string? name) {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");
        Name = name.Trim();
    }

    public void ChangePrice(Money price) {
        Price = price ?? throw new DomainException("Price is required.");
    }

    public void MakeAvailable() { Status = MenuStatus.Available; Touch(); }
    public void MakeUnavailable() { Status = MenuStatus.Unavailable; Touch(); }

    public bool IsAvailable => Status == MenuStatus.Available;
}
