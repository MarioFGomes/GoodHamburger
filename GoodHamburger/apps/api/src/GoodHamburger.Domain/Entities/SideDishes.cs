using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Domain.Entities; 
public class SideDishes: EntityBase {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public SideDishCategory Category { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public MenuStatus Status { get; set; } = MenuStatus.Available;
}
