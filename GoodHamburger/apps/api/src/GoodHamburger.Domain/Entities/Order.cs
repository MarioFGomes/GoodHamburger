using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;
public class Order : EntityBase {
    public Guid CustomerID { get; set; }
    public int OrderNumber { get; set; }
    public decimal Total { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Subtotal { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.PENDING;
    public virtual Customer Customer {get;set; }

    private readonly List<OrderItem> _OrderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _OrderItems;

    protected Order() { }

    public void AddSandwich(Guid menuId, decimal unitPrice) {
       
        EnsurePending();

        if (_OrderItems.Any())
            throw new DomainException("Este pedido já contém um sanduíche. Não é permitido duplicar.");

        _OrderItems.Add(new OrderItem(menuId, unitPrice));
        RecalculateTotals();
    }


    private void EnsurePending() {
        if (Status != OrderStatus.PENDING)
            throw new DomainException($"Operação não permitida: pedido está {Status}.");
    }

    private decimal CalculateComboDiscount() {
       
        if (!_OrderItems.Any()) return 0m;

        var sandwich = _OrderItems.First();
        var hasFries = sandwich.HasFries();
        var hasDrink = sandwich.HasDrink();

        if (hasFries && hasDrink) return 20m;  
        if (hasDrink) return 15m;  
        if (hasFries) return 10m;  
        return 0m;                             
    }


    private void RecalculateTotals() {
        Subtotal = _OrderItems.Sum(i => i.CalculateTotal());
        Discount = CalculateComboDiscount();
        var discountAmount = Subtotal * (Discount / 100m);
        Total = Subtotal - discountAmount;
    }


    public void Confirm() {
        EnsurePending();
        if (!_OrderItems.Any()) throw new DomainException("Não é possível confirmar um pedido vazio.");

        Status = OrderStatus.CONFIRMED;
    }


    public void Cancel() {
        if (Status == OrderStatus.DELIVERED)
            throw new DomainException("Pedido já entregue não pode ser cancelado.");
        if (Status == OrderStatus.CANCELLED)
            throw new DomainException("Pedido já está cancelado.");

        Status = OrderStatus.CANCELLED;
        UpdatedAt = DateTime.UtcNow;
    }

}
