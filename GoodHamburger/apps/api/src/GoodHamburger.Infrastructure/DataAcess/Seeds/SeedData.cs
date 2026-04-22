using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Seeds;
public class SeedData {

    public static void Seed(ModelBuilder modelBuilder) {
       
        var menu1 = new Menu {
            Name = "X Burger",
            Description = "",
            Price = 5m

        };

        var menu2 = new Menu {
            Name = "X Egg",
            Description = "",
            Price = 4.50m

        };


        var menu3 = new Menu {
            Name = "X Bacon",
            Description = "",
            Price = 7m
        };

        modelBuilder.Entity<Menu>().HasData(menu1, menu2,menu3);



        var sideDishes1 = new SideDishes {
            Name = "Batata Frita",
            Description = "",
            Price = 2m,
            Category= SideDishCategory.FRIES

        };


        var sideDishes2 = new SideDishes {
            Name = "Refrigerante",
            Description = "",
            Price = 2.50m,
            Category= SideDishCategory.DRINK

        };

        modelBuilder.Entity<SideDishes>().HasData(sideDishes1, sideDishes2);


        var Customer1 = new Customer {
          FirstName="Lucas",
          LastName="Silva",
          Address="Rio de Janeiro",
          Phone="+55 21 97534-2254"
        };

        modelBuilder.Entity<Customer>().HasData(Customer1);
    }

}
