using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Seeds;
public class SeedData {

    public static void Seed(ModelBuilder modelBuilder) {

        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Menu>().HasData(
            new Menu {
                Id          = new Guid("a1000000-0000-0000-0000-000000000001"),
                Name        = "X Burger",
                Description = "Pão, hambúrguer artesanal e queijo",
                Price       = 5m,
                Currency    = Currency.BRL,
                Status      = MenuStatus.Available,
                CreatedAt   = seedDate,
                UpdatedAt   = seedDate
            },
            new Menu {
                Id          = new Guid("a1000000-0000-0000-0000-000000000002"),
                Name        = "X Egg",
                Description = "Pão, hambúrguer artesanal, queijo e ovo",
                Price       = 4.50m,
                Currency    = Currency.BRL,
                Status      = MenuStatus.Available,
                CreatedAt   = seedDate,
                UpdatedAt   = seedDate
            },
            new Menu {
                Id          = new Guid("a1000000-0000-0000-0000-000000000003"),
                Name        = "X Bacon",
                Description = "Pão, hambúrguer artesanal, queijo e bacon",
                Price       = 7m,
                Currency    = Currency.BRL,
                Status      = MenuStatus.Available,
                CreatedAt   = seedDate,
                UpdatedAt   = seedDate
            }
        );

        modelBuilder.Entity<SideDishes>().HasData(
            new SideDishes {
                Id          = new Guid("b2000000-0000-0000-0000-000000000001"),
                Name        = "Batata Frita",
                Description = "Porção de batata frita crocante",
                Price       = 2m,
                Category    = SideDishCategory.FRIES,
                Currency    = Currency.BRL,
                Status      = MenuStatus.Available,
                CreatedAt   = seedDate,
                UpdatedAt   = seedDate
            },
            new SideDishes {
                Id          = new Guid("b2000000-0000-0000-0000-000000000002"),
                Name        = "Refrigerante",
                Description = "Lata 350ml",
                Price       = 2.50m,
                Category    = SideDishCategory.DRINK,
                Currency    = Currency.BRL,
                Status      = MenuStatus.Available,
                CreatedAt   = seedDate,
                UpdatedAt   = seedDate
            }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer {
                Id        = new Guid("c3000000-0000-0000-0000-000000000001"),
                FirstName = "Lucas",
                LastName  = "Silva",
                Address   = "Rio de Janeiro",
                Phone     = "+55 21 97534-2254",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
