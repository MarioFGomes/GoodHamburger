using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Domain.Entities;
public class Order : EntityBase {
    public Guid CustomerID { get; set; }
    public int OrderNumber { get; set; }
    public decimal Total { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Subtotal { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.PENDING;
    public virtual Customer customer {get;set; }

    private readonly List<OrderItem> _OrderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _OrderItems;


    public void MakeOrder() 
    {

    }

}
