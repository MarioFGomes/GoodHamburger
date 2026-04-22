using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Mappers;
public static class MenuMapper {

    public static MenuResponse ToResponse(this Menu menu) {
        return new MenuResponse {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            Price = menu.Price,
            Currency = menu.Currency,
            Status = menu.Status,
        };
    }

    public static Menu ToDomain(this CreateMenuRequest request) {
        return new Menu {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
        };
    }

    public static Menu ToDomain(this UpdateMenuRequest request) {
        return new Menu {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Currency = request.Currency,
            Status = request.Status,
        };
    }
}
