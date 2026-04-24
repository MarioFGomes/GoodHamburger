using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Application.DTOs.Responses;
public class OrderSideDishResponse {
    public Guid SideDishId { get; set; }
    public string Name { get; set; } = string.Empty;
    public SideDishCategory Category { get; set; }
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
}
