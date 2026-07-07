using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.StateMachines;

namespace DomainTest.Orders;

public class OrderStateMachineTest {

    [Theory]
    [InlineData(OrderStatus.PENDING, OrderStatus.CONFIRMED, true)]
    [InlineData(OrderStatus.PENDING, OrderStatus.CANCELLED, true)]
    [InlineData(OrderStatus.PENDING, OrderStatus.PAID, false)]
    [InlineData(OrderStatus.PENDING, OrderStatus.DELIVERED, false)]
    [InlineData(OrderStatus.CONFIRMED, OrderStatus.PAID, true)]
    [InlineData(OrderStatus.CONFIRMED, OrderStatus.CANCELLED, true)]
    [InlineData(OrderStatus.CONFIRMED, OrderStatus.READY, false)]
    [InlineData(OrderStatus.PAID, OrderStatus.READY, true)]
    [InlineData(OrderStatus.PAID, OrderStatus.CANCELLED, false)]
    [InlineData(OrderStatus.READY, OrderStatus.DELIVERED, true)]
    [InlineData(OrderStatus.DELIVERED, OrderStatus.CANCELLED, false)]
    [InlineData(OrderStatus.CANCELLED, OrderStatus.CONFIRMED, false)]
    public void CanTransition_MatchesTheLifecycleTable(OrderStatus from, OrderStatus to, bool allowed) {
        OrderStateMachine.CanTransition(from, to).Should().Be(allowed);
    }

    [Fact]
    public void TerminalStates_HaveNoOutgoingTransitions() {
        OrderStateMachine.AllowedFrom(OrderStatus.DELIVERED).Should().BeEmpty();
        OrderStateMachine.AllowedFrom(OrderStatus.CANCELLED).Should().BeEmpty();
    }

    [Fact]
    public void Order_FullLifecycle_ReachesDelivered() {
        var order = NewOrderWithSandwich();

        order.Confirm();
        order.Pay();
        order.MarkReady();
        order.Deliver();

        order.Status.Should().Be(OrderStatus.DELIVERED);
    }

    [Fact]
    public void Order_PayBeforeConfirm_Throws() {
        var order = NewOrderWithSandwich();
        var act = order.Pay;
        act.Should().Throw<DomainException>().WithMessage("*transition*PENDING*PAID*");
    }

    [Fact]
    public void Order_CancelAfterPayment_IsNotAllowed() {
        var order = NewOrderWithSandwich();
        order.Confirm();
        order.Pay();

        var act = order.Cancel;
        act.Should().Throw<DomainException>().WithMessage("*transition*");
    }

    [Fact]
    public void Order_DeliverTwice_Throws() {
        var order = NewOrderWithSandwich();
        order.Confirm();
        order.Pay();
        order.MarkReady();
        order.Deliver();

        var act = order.Deliver;
        act.Should().Throw<DomainException>();
    }

    private static Order NewOrderWithSandwich() {
        var order = new Order(Guid.NewGuid(), 1);
        order.AddSandwich(Guid.NewGuid(), 10m);
        return order;
    }
}
