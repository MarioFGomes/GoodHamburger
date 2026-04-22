using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Application.DTOs.Requests;
public class CreateMenuRequest {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
}
