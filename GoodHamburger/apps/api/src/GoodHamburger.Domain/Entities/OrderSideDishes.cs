using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities; 
public class OrderSideDishes {
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid SideDishesId { get; set; }
    public int Qtd { get; set; }
    public SideDishCategory Category { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual OrderItem OrderItem { get; set; }

    protected OrderSideDishes()
    {}

    public OrderSideDishes(Guid sideDishesId, SideDishCategory category, decimal unitPrice) {
        
        if (unitPrice < 0) throw new DomainException("Preço não pode ser negativo.");

        SideDishesId = sideDishesId;
        Category = category;
        Qtd = 1;                    
        UnitPrice = unitPrice;
        Id=Guid.NewGuid();
    }
    public decimal CalculateTotal() => UnitPrice * Qtd;
}
