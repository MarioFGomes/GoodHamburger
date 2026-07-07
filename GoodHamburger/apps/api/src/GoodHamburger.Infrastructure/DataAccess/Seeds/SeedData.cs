using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Infrastructure.DataAccess.Seeds;

/// <summary>
/// Runtime catalog seeding. Runs at startup and only inserts when the tables
/// are empty, so it is safe for existing databases (which already carry the
/// rows inserted by the historical SeedInitialData migration).
/// </summary>
public static class SeedData {

    public static void EnsureSeeded(GoodHamburgerContext context) {

        if (!context.Set<Menu>().Any()) {
            context.Set<Menu>().AddRange(
                new Menu("X Burger", "Pão, hambúrguer artesanal e queijo", Money.Create(5m)),
                new Menu("X Egg", "Pão, hambúrguer artesanal, queijo e ovo", Money.Create(4.50m)),
                new Menu("X Bacon", "Pão, hambúrguer artesanal, queijo e bacon", Money.Create(7m)));
        }

        if (!context.Set<SideDishes>().Any()) {
            context.Set<SideDishes>().AddRange(
                new SideDishes("Batata Frita", "Porção de batata frita crocante", Money.Create(2m), SideDishCategory.FRIES),
                new SideDishes("Refrigerante", "Lata 350ml", Money.Create(2.50m), SideDishCategory.DRINK));
        }

        context.SaveChanges();
    }
}
