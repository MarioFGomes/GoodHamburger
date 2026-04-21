namespace GoodHamburger.Domain.Entities; 
public class OrderSideDishes: EntityBase {
    public Guid OrderItemId { get; set; }
    public Guid SideDishesId { get; set; }
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual OrderItem OrderItem { get; set; }

    protected OrderSideDishes()
    {}
}
