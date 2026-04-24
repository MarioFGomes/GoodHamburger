namespace DomainTest.Orders;

public class OrderSideDishesEntityTest {

    #region Constructor

    [Fact]
    public void Constructor_ValidArgs_SetsProperties() {
        var sideDishId = Guid.NewGuid();

        var result = new OrderSideDishes(sideDishId, SideDishCategory.FRIES, 5.00m);

        result.SideDishesId.Should().Be(sideDishId);
        result.Category.Should().Be(SideDishCategory.FRIES);
        result.UnitPrice.Should().Be(5.00m);
        result.Qtd.Should().Be(1);
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_NegativePrice_ThrowsDomainException() {
        var act = () => new OrderSideDishes(Guid.NewGuid(), SideDishCategory.FRIES, -0.01m);

        act.Should().Throw<DomainException>()
           .WithMessage("*negativo*");
    }

    [Fact]
    public void Constructor_ZeroPrice_CreatesSuccessfully() {
        var act = () => new OrderSideDishes(Guid.NewGuid(), SideDishCategory.DRINK, 0m);

        act.Should().NotThrow();
    }

    #endregion

    #region CalculateTotal

    [Fact]
    public void CalculateTotal_ReturnsUnitPriceTimesQtd() {
        var sideDish = new OrderSideDishes(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        sideDish.CalculateTotal().Should().Be(4.00m);
    }

    [Fact]
    public void CalculateTotal_FriesPrice_ReturnsCorrectValue() {
        var sideDish = new OrderSideDishes(Guid.NewGuid(), SideDishCategory.FRIES, 8.50m);

        sideDish.CalculateTotal().Should().Be(8.50m);
    }

    #endregion
}
