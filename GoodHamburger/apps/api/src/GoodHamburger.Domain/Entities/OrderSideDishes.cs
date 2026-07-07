using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;
public class OrderSideDishes : EntityBase {
    public Guid OrderItemId { get; set; }
    public Guid SideDishesId { get; set; }
    public int Quantity { get; set; }
    public SideDishCategory Category { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual OrderItem OrderItem { get; set; } = null!;
    public virtual SideDishes? SideDishes { get; set; }

    protected OrderSideDishes() { }

    public OrderSideDishes(Guid sideDishesId, SideDishCategory category, decimal unitPrice) {

        if (unitPrice < 0) throw new DomainException("Price cannot be negative.");

        SideDishesId = sideDishesId;
        Category = category;
        Quantity = 1;
        UnitPrice = unitPrice;
    }
    public decimal CalculateTotal() => UnitPrice * Quantity;
}
