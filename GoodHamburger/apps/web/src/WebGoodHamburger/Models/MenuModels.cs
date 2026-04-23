using System.ComponentModel.DataAnnotations;
using WebGoodHamburger.Models.Enums;

namespace WebGoodHamburger.Models;

public class MenuResponse {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public Currency Currency { get; set; }
    public MenuStatus Status { get; set; }
}

public class CreateMenuRequest {
    [Required] [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")] public decimal Price { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
}

public class UpdateMenuRequest {
    [Required] [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    [Required] [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")] public decimal Price { get; set; }
    public Currency Currency { get; set; } = Currency.BRL;
    public MenuStatus Status { get; set; } = MenuStatus.Available;
}
