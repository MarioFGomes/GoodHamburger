namespace DomainTest.Orders;

public class OrderEntityTest {

    private static Order BuildOrderWithSandwich(decimal sandwichPrice = 20.00m) {
        var order = new Order(Guid.NewGuid(), 1);
        order.AddSandwich(Guid.NewGuid(), sandwichPrice);
        return order;
    }

    #region Constructor

    [Fact]
    public void Constructor_SetsProperties_StatusIsPending() {
        var customerId = Guid.NewGuid();

        var order = new Order(customerId, 42);

        order.CustomerID.Should().Be(customerId);
        order.OrderNumber.Should().Be(42);
        order.Status.Should().Be(OrderStatus.PENDING);
        order.Subtotal.Should().Be(0);
        order.Discount.Should().Be(0);
        order.Total.Should().Be(0);
        order.OrderItems.Should().BeEmpty();
    }

    #endregion

    #region AddSandwich

    [Fact]
    public void AddSandwich_Success_CalculatesTotalsAndAddsItem() {
        var order = new Order(Guid.NewGuid(), 1);

        order.AddSandwich(Guid.NewGuid(), 20.00m);

        order.OrderItems.Should().HaveCount(1);
        order.Subtotal.Should().Be(20.00m);
        order.Total.Should().Be(20.00m);
        order.Discount.Should().Be(0);
    }

    [Fact]
    public void AddSandwich_Twice_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();

        var act = () => order.AddSandwich(Guid.NewGuid(), 20.00m);

        act.Should().Throw<DomainException>()
           .WithMessage("*sanduíche*");
    }

    [Fact]
    public void AddSandwich_WhenConfirmed_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Confirm();

        var act = () => order.AddSandwich(Guid.NewGuid(), 20.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddSandwich_WhenCancelled_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Cancel();

        var act = () => order.AddSandwich(Guid.NewGuid(), 20.00m);

        act.Should().Throw<DomainException>();
    }

    #endregion

    #region AddSideDish — pré-condições

    [Fact]
    public void AddSideDish_BeforeSandwich_ThrowsDomainException() {
        var order = new Order(Guid.NewGuid(), 1);

        var act = () => order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        act.Should().Throw<DomainException>()
           .WithMessage("*sanduíche*");
    }

    [Fact]
    public void AddSideDish_WhenConfirmed_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Confirm();

        var act = () => order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddSideDish_WhenCancelled_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Cancel();

        var act = () => order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddSideDish_DuplicateFRIES_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        var act = () => order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        act.Should().Throw<DomainException>();
    }

    #endregion

    #region AddSideDish — cálculo de descontos

    [Fact]
    public void AddSideDish_FRIES_Applies10PercentDiscount() {
        var order = BuildOrderWithSandwich(20.00m);

        order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        order.Subtotal.Should().Be(25.00m);
        order.Discount.Should().Be(10m);
        order.Total.Should().Be(22.50m);
    }

    [Fact]
    public void AddSideDish_DRINK_Applies15PercentDiscount() {
        var order = BuildOrderWithSandwich(20.00m);

        order.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        order.Subtotal.Should().Be(24.00m);
        order.Discount.Should().Be(15m);
        order.Total.Should().Be(20.40m);
    }

    [Fact]
    public void AddSideDish_Combo_Applies20PercentDiscount() {
        var order = BuildOrderWithSandwich(20.00m);
        order.AddSideDish(Guid.NewGuid(), SideDishCategory.FRIES, 5.00m);

        order.AddSideDish(Guid.NewGuid(), SideDishCategory.DRINK, 4.00m);

        order.Subtotal.Should().Be(29.00m);
        order.Discount.Should().Be(20m);
        order.Total.Should().Be(23.20m);
    }

    [Fact]
    public void AddSandwich_NoSideDishes_ZeroDiscount() {
        var order = BuildOrderWithSandwich(30.00m);

        order.Discount.Should().Be(0);
        order.Total.Should().Be(order.Subtotal);
    }

    #endregion

    #region Confirm

    [Fact]
    public void Confirm_Success_StatusIsConfirmed() {
        var order = BuildOrderWithSandwich();

        order.Confirm();

        order.Status.Should().Be(OrderStatus.CONFIRMED);
    }

    [Fact]
    public void Confirm_EmptyOrder_ThrowsDomainException() {
        var order = new Order(Guid.NewGuid(), 1);

        var act = () => order.Confirm();

        act.Should().Throw<DomainException>()
           .WithMessage("*vazio*");
    }

    [Fact]
    public void Confirm_AlreadyConfirmed_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Confirm();

        var act = () => order.Confirm();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Confirm_WhenCancelled_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Cancel();

        var act = () => order.Confirm();

        act.Should().Throw<DomainException>();
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_WhenPending_StatusIsCancelled() {
        var order = BuildOrderWithSandwich();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.CANCELLED);
    }

    [Fact]
    public void Cancel_WhenConfirmed_StatusIsCancelled() {
        var order = BuildOrderWithSandwich();
        order.Confirm();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.CANCELLED);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException() {
        var order = BuildOrderWithSandwich();
        order.Cancel();

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>()
           .WithMessage("*cancelado*");
    }

    #endregion
}
