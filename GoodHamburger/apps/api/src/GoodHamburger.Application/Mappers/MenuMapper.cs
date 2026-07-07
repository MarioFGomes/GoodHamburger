using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Application.Mappers;
public static class MenuMapper {

    public static MenuResponse ToResponse(this Menu menu) {
        return new MenuResponse {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            Price = menu.Price.Amount,
            Currency = menu.Price.Currency,
            Status = menu.Status,
        };
    }

    public static Menu ToDomain(this CreateMenuRequest request) {
        return new Menu(
            request.Name,
            request.Description,
            Money.Create(request.Price ?? 0m, request.Currency));
    }
}
