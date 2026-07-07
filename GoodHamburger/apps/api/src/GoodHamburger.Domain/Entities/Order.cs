using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.StateMachines;

namespace GoodHamburger.Domain.Entities;
public class Order : EntityBase {
    public Guid CustomerID { get; set; }
    public int OrderNumber { get; set; }
    public decimal Total { get; private set; }

    /// <summary>Combo discount applied to the subtotal, in percent (0, 10, 15 or 20).</summary>
    public decimal DiscountPercentage { get; private set; }
    public decimal Subtotal { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.PENDING;

    /// <summary>Client-supplied key that makes order creation retry-safe.</summary>
    public string? IdempotencyKey { get; private set; }

    public virtual Customer Customer { get; set; } = null!;

    private readonly List<OrderItem> _OrderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _OrderItems;

    protected Order() { }

    public Order(Guid customerId, int orderNumber, string? idempotencyKey = null) {
        CustomerID = customerId;
        OrderNumber = orderNumber;
        SetIdempotencyKey(idempotencyKey);
    }

    public void SetIdempotencyKey(string? key) {
        if (key is not null && key.Length > 64)
            throw new DomainException("Idempotency key cannot exceed 64 characters.");
        IdempotencyKey = string.IsNullOrWhiteSpace(key) ? null : key.Trim();
    }

    public void AddSideDish(Guid sideDishId, SideDishCategory category, decimal unitPrice) {
        EnsurePending();
        if (!_OrderItems.Any())
            throw new DomainException("Add a sandwich before adding side dishes.");
        _OrderItems.First().AddSideDish(sideDishId, category, unitPrice);
        RecalculateTotals();
    }

    public void AddSandwich(Guid menuId, decimal unitPrice) {
        EnsurePending();

        if (_OrderItems.Any())
            throw new DomainException("This order already contains a sandwich. Duplicates are not allowed.");

        _OrderItems.Add(new OrderItem(menuId, unitPrice));
        RecalculateTotals();
    }

    private void EnsurePending() {
        if (Status != OrderStatus.PENDING)
            throw new DomainException($"Operation not allowed: order is {Status}.");
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
        DiscountPercentage = CalculateComboDiscount();
        var discountAmount = Subtotal * (DiscountPercentage / 100m);
        Total = Subtotal - discountAmount;
    }

    /// <summary>
    /// All status changes go through the <see cref="OrderStateMachine"/>,
    /// which is the single place that knows the valid transitions.
    /// </summary>
    private void TransitionTo(OrderStatus next) {
        OrderStateMachine.EnsureTransition(Status, next);
        Status = next;
        Touch();
    }

    public void Confirm() {
        if (!_OrderItems.Any())
            throw new DomainException("An empty order cannot be confirmed.");
        TransitionTo(OrderStatus.CONFIRMED);
    }

    public void Pay() => TransitionTo(OrderStatus.PAID);

    public void MarkReady() => TransitionTo(OrderStatus.READY);

    public void Deliver() => TransitionTo(OrderStatus.DELIVERED);

    public void Cancel() => TransitionTo(OrderStatus.CANCELLED);
}
