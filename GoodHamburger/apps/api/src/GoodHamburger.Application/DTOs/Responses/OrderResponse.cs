using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Application.DTOs.Responses;
public class OrderResponse {
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}
