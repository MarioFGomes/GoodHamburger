using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Domain.Entities;
public class SideDishes : EntityBase {

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = null!;
    public SideDishCategory Category { get; private set; }
    public MenuStatus Status { get; private set; } = MenuStatus.Available;

    protected SideDishes() { }

    public SideDishes(string? name, string? description, Money price, SideDishCategory category) {
        Rename(name);
        Description = description;
        ChangePrice(price);
        Category = category;
    }

    public void Update(string? name, string? description, Money price, SideDishCategory category, MenuStatus status) {
        Rename(name);
        Description = description;
        ChangePrice(price);
        Category = category;
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
