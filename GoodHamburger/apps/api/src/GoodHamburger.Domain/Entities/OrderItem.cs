using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;
public class OrderItem : EntityBase {
    public Guid OrderId { get; set; }
    public Guid MenuId { get; set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual Menu Menu { get; set; } = null!;

    private readonly List<OrderSideDishes> _OrderSideDishes = new();
    public IReadOnlyCollection<OrderSideDishes> OrderSideDishes => _OrderSideDishes;

    protected OrderItem() { }

    public OrderItem(Guid menuId, decimal unitPrice) {
        if (unitPrice < 0) throw new DomainException("Price cannot be negative.");
        MenuId = menuId;
        Quantity = 1;
        UnitPrice = unitPrice;
    }

    public void AddSideDish(Guid sideDishesId, SideDishCategory category, decimal unitPrice) {

        var alreadyHasCategory = _OrderSideDishes.Any(s => s.Category == category);

        if (alreadyHasCategory) {
            var name = category == SideDishCategory.FRIES ? "fries" : "drink";
            throw new DomainException(
                $"This order already contains {name}. Duplicate side dishes are not allowed.");
        }

        _OrderSideDishes.Add(new OrderSideDishes(sideDishesId, category, unitPrice));
    }

    public bool HasFries() => _OrderSideDishes.Any(s => s.Category == SideDishCategory.FRIES);
    public bool HasDrink() => _OrderSideDishes.Any(s => s.Category == SideDishCategory.DRINK);

    public decimal CalculateTotal() {
        var sideDishesTotal = _OrderSideDishes.Sum(sd => sd.CalculateTotal());
        return (UnitPrice * Quantity) + sideDishesTotal;
    }
}
