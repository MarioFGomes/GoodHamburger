using Bogus;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;

namespace Utils.Entities;
public class MenuBuilder {

    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public Currency Currency { get; private set; }

    public static MenuBuilder Create() {
        var faker = new Faker("pt_BR");
        return new MenuBuilder {
            Name = faker.Commerce.ProductName(),
            Description = faker.Lorem.Sentence(),
            Price = faker.Random.Decimal(5, 50),
            Currency = Currency.BRL
        };
    }

    public static List<MenuBuilder> CreateMany(int count) {
        var list = new List<MenuBuilder>();
        for (var i = 0; i < count; i++)
            list.Add(Create());
        return list;
    }

    public Menu ToEntity() {
        return new Menu {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Price = Price,
            Currency = Currency,
            Status = MenuStatus.Available
        };
    }

    public CreateMenuRequest ToRequest() {
        return new CreateMenuRequest {
            Name = Name,
            Description = Description,
            Price = Price,
            Currency = Currency
        };
    }

    public UpdateMenuRequest ToUpdateRequest() {
        return new UpdateMenuRequest {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Price = Price,
            Currency = Currency,
            Status = MenuStatus.Available
        };
    }
}
