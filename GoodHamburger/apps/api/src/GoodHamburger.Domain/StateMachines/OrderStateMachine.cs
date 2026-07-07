using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.StateMachines;

/// <summary>
/// Single source of truth for the order lifecycle. Every status change in
/// <see cref="Entities.Order"/> goes through this table, so adding or
/// restricting a transition is a one-line change reviewed in one place.
///
///   PENDING -> CONFIRMED -> PAID -> READY -> DELIVERED
///      |            |
///      +------------+--> CANCELLED
/// </summary>
public static class OrderStateMachine {

    private static readonly IReadOnlyDictionary<OrderStatus, OrderStatus[]> Transitions =
        new Dictionary<OrderStatus, OrderStatus[]> {
            [OrderStatus.PENDING]   = new[] { OrderStatus.CONFIRMED, OrderStatus.CANCELLED },
            [OrderStatus.CONFIRMED] = new[] { OrderStatus.PAID, OrderStatus.CANCELLED },
            [OrderStatus.PAID]      = new[] { OrderStatus.READY },
            [OrderStatus.READY]     = new[] { OrderStatus.DELIVERED },
            [OrderStatus.DELIVERED] = Array.Empty<OrderStatus>(),
            [OrderStatus.CANCELLED] = Array.Empty<OrderStatus>(),
        };

    public static bool CanTransition(OrderStatus from, OrderStatus to) =>
        Transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static void EnsureTransition(OrderStatus from, OrderStatus to) {
        if (!CanTransition(from, to))
            throw new DomainException($"Invalid order status transition: {from} -> {to}.");
    }

    public static IReadOnlyCollection<OrderStatus> AllowedFrom(OrderStatus from) =>
        Transitions.TryGetValue(from, out var allowed) ? allowed : Array.Empty<OrderStatus>();
}
