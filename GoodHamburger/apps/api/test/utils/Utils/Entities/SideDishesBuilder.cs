using Bogus;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;

namespace Utils.Entities;
public class SideDishesBuilder {

    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public SideDishCategory Category { get; private set; }
    public Currency Currency { get; private set; }

    public static SideDishesBuilder Create(SideDishCategory? category = null) {
        var faker = new Faker("pt_BR");
        return new SideDishesBuilder {
            Name = faker.Commerce.ProductName(),
            Description = faker.Lorem.Sentence(),
            Price = faker.Random.Decimal(2, 15),
            Category = category ?? faker.PickRandom<SideDishCategory>(),
            Currency = Currency.BRL
        };
    }

    public static SideDishesBuilder CreateFries() => Create(SideDishCategory.FRIES);
    public static SideDishesBuilder CreateDrink() => Create(SideDishCategory.DRINK);

    public static List<SideDishesBuilder> CreateMany(int count) {
        var list = new List<SideDishesBuilder>();
        for (var i = 0; i < count; i++)
            list.Add(Create());
        return list;
    }

    public SideDishes ToEntity() {
        return new SideDishes {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Price = Price,
            Category = Category,
            Currency = Currency,
            Status = MenuStatus.Available
        };
    }

    public CreateSideDishesRequest ToRequest() {
        return new CreateSideDishesRequest {
            Name = Name,
            Description = Description,
            Price = Price,
            Category = Category,
            Currency = Currency
        };
    }

    public UpdateSideDishesRequest ToUpdateRequest() {
        return new UpdateSideDishesRequest {
            Id = Guid.NewGuid(),
            Name = Name,
            Description = Description,
            Price = Price,
            Category = Category,
            Currency = Currency,
            Status = MenuStatus.Available
        };
    }
}
