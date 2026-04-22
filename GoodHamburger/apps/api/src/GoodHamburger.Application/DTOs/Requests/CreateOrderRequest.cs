namespace GoodHamburger.Application.DTOs.Requests;
public class CreateOrderRequest {
    public Guid CustomerId { get; set; }
    public Guid MenuId { get; set; }
    public List<Guid> SideDishIds { get; set; } = new();
}
