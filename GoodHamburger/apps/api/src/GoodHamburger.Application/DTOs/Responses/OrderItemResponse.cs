namespace GoodHamburger.Application.DTOs.Responses;
public class OrderItemResponse {
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
    public List<OrderSideDishResponse> SideDishes { get; set; } = new();
}
