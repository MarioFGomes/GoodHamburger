using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;
public class Menu: EntityBase {

    private string? _name;
    private decimal? _price;

    public string? Name {
        get => _name;
        set {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new DomainException("Name não pode ser vazio.");
            _name = value;
        }
    }
    public string? Description { get; set; }
    public decimal? Price {
        get => _price;
        set {
            if (value < 0)
                throw new DomainException("Preço não pode ser negativo.");
            _price = value;
        }
    }
    public Currency Currency { get; set; } = Currency.BRL;
    public MenuStatus Status { get; set; } = MenuStatus.Available;

}
