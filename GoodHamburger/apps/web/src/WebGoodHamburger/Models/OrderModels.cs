using System.ComponentModel.DataAnnotations;
using WebGoodHamburger.Models.Enums;

namespace WebGoodHamburger.Models;

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

public class OrderItemResponse {
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
    public List<OrderSideDishResponse> SideDishes { get; set; } = new();
}

public class OrderSideDishResponse {
    public Guid SideDishId { get; set; }
    public SideDishCategory Category { get; set; }
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderRequest {
    [Required] public Guid CustomerId { get; set; }
    [Required] public Guid MenuId { get; set; }
    public List<Guid> SideDishIds { get; set; } = new();
}
