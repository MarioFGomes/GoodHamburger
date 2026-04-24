using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;

namespace Utils.Entities;
public class OrderBuilder {

    public static Order Create() {
        var order = new Order(Guid.NewGuid(), 1);
        order.AddSandwich(Guid.NewGuid(), 20.00m);
        return order;
    }

    public static Order CreateWithFries() {
        var order = Create();
        order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);
        return order;
    }

    public static Order CreateWithDrink() {
        var order = Create();
        order.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);
        return order;
    }

    public static Order CreateWithCombo() {
        var order = CreateWithFries();
        order.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);
        return order;
    }
}
