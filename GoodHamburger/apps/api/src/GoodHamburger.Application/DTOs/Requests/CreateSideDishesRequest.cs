using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Application.DTOs.Requests;
public class CreateSideDishesRequest {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public SideDishCategory Category { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
}
