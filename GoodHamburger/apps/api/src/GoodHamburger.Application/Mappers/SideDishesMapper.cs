using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Application.Mappers;
public static class SideDishesMapper {

    public static SideDishesResponse ToResponse(this SideDishes sideDish) {
        return new SideDishesResponse {
            Id = sideDish.Id,
            Name = sideDish.Name,
            Description = sideDish.Description,
            Price = sideDish.Price.Amount,
            Currency = sideDish.Price.Currency,
            Category = sideDish.Category,
            Status = sideDish.Status,
        };
    }

    public static SideDishes ToDomain(this CreateSideDishesRequest request) {
        return new SideDishes(
            request.Name,
            request.Description,
            Money.Create(request.Price ?? 0m, request.Currency),
            request.Category);
    }
}
