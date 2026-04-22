using GoodHamburger.Domain.Enum;

namespace GoodHamburger.Application.DTOs.Responses;
public class SideDishesResponse {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public SideDishCategory Category { get; set; }
    public Currency Currency { get; set; }
    public MenuStatus Status { get; set; }
}
