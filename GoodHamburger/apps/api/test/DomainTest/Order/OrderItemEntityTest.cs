namespace DomainTest.Orders;

public class OrderItemEntityTest {

    #region Constructor

    [Fact]
    public void Constructor_ValidPrice_SetsQtdToOneAndAssignsId() {
        var menuId = Guid.NewGuid();

        var item = new OrderItem(menuId, 20.00m);

        item.MenuId.Should().Be(menuId);
        item.UnitPrice.Should().Be(20.00m);
        item.Qtd.Should().Be(1);
        item.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_NegativePrice_ThrowsDomainException() {
        var act = () => new OrderItem(Guid.NewGuid(), -0.01m);

        act.Should().Throw<DomainException>()
           .WithMessage("*negativo*");
    }

    [Fact]
    public void Constructor_ZeroPrice_CreatesSuccessfully() {
        var act = () => new OrderItem(Guid.NewGuid(), 0m);

        act.Should().NotThrow();
    }

    #endregion

    #region HasFries / HasDrink

    [Fact]
    public void HasFries_WithoutSideDishes_ReturnsFalse() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);

        item.HasFries().Should().BeFalse();
    }

    [Fact]
    public void HasDrink_WithoutSideDishes_ReturnsFalse() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);

        item.HasDrink().Should().BeFalse();
    }

    [Fact]
    public void HasFries_AfterAddingFries_ReturnsTrue() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        item.HasFries().Should().BeTrue();
        item.HasDrink().Should().BeFalse();
    }

    [Fact]
    public void HasDrink_AfterAddingDrink_ReturnsTrue() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        item.HasDrink().Should().BeTrue();
        item.HasFries().Should().BeFalse();
    }

    #endregion

    #region AddSideDish

    [Fact]
    public void AddSideDish_FRIESThenDRINK_BothCategoriesAllowed() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        item.HasFries().Should().BeTrue();
        item.HasDrink().Should().BeTrue();
        item.OrderSideDishes.Should().HaveCount(2);
    }

    [Fact]
    public void AddSideDish_DuplicateFRIES_ThrowsDomainException() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        var act = () => item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        act.Should().Throw<DomainException>()
           .WithMessage("*batata frita*");
    }

    [Fact]
    public void AddSideDish_DuplicateDRINK_ThrowsDomainException() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        var act = () => item.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        act.Should().Throw<DomainException>()
           .WithMessage("*refrigerante*");
    }

    #endregion

    #region CalculateTotal

    [Fact]
    public void CalculateTotal_WithoutSideDishes_ReturnsUnitPrice() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);

        item.CalculateTotal().Should().Be(20.00m);
    }

    [Fact]
    public void CalculateTotal_WithFries_IncludesSideDishPrice() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        item.CalculateTotal().Should().Be(25.00m);
    }

    [Fact]
    public void CalculateTotal_WithFriesAndDrink_IncludesAllPrices() {
        var item = new OrderItem(Guid.NewGuid(), 20.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);
        item.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        item.CalculateTotal().Should().Be(29.00m);
    }

    #endregion
}
