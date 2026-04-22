using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Mappers;
public static class SideDishesMapper {

    public static SideDishesResponse ToResponse(this SideDishes sideDish) {
        return new SideDishesResponse {
            Id = sideDish.Id,
            Name = sideDish.Name,
            Description = sideDish.Description,
            Price = sideDish.Price,
            Category = sideDish.Category,
            Currency = sideDish.Currency,
            Status = sideDish.Status,
        };
    }

    public static SideDishes ToDomain(this CreateSideDishesRequest request) {
        return new SideDishes {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Currency = request.Currency,
        };
    }

    public static SideDishes ToDomain(this UpdateSideDishesRequest request) {
        return new SideDishes {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Currency = request.Currency,
            Status = request.Status,
        };
    }
}
