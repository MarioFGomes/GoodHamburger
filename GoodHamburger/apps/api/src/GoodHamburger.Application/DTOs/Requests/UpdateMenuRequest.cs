using GoodHamburger.Domain.Enum;
using System.Text.Json.Serialization;

namespace GoodHamburger.Application.DTOs.Requests;
public class UpdateMenuRequest {
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public MenuStatus Status { get; set; } = MenuStatus.available;
}
