using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Mappers;
public static class OrderMapper {

    public static OrderResponse ToResponse(this Order order) {
        return new OrderResponse {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerID,
            CustomerName = order.Customer is not null
                ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                : string.Empty,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Total = order.Total,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => i.ToResponse()).ToList(),
        };
    }

    private static OrderItemResponse ToResponse(this OrderItem item) {
        return new OrderItemResponse {
            Id = item.Id,
            MenuId = item.MenuId,
            MenuName = item.Menu?.Name ?? string.Empty,
            Qtd = item.Qtd,
            UnitPrice = item.UnitPrice,
            SideDishes = item.OrderSideDishes.Select(s => s.ToResponse()).ToList(),
        };
    }

    private static OrderSideDishResponse ToResponse(this OrderSideDishes sideDish) {
        return new OrderSideDishResponse {
            SideDishId = sideDish.SideDishesId,
            Name = sideDish.SideDishes?.Name ?? string.Empty,
            Category = sideDish.Category,
            Qtd = sideDish.Qtd,
            UnitPrice = sideDish.UnitPrice,
        };
    }
}
