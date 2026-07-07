using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Application.Mappers;
public static class CustomerMapper {

    public static CustomerResponse ToResponse(this Customer customer) {
        return new CustomerResponse {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Address = customer.Address,
            Phone = customer.Phone?.Value,
            Email = customer.Email?.Value,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
        };
    }

    public static Customer ToDomain(this CreateCustomerRequest request) {
        return new Customer(
            request.FirstName,
            request.LastName,
            Email.Create(request.Email),
            Phone.Create(request.Phone),
            request.Address);
    }

    public static CreateCustomerRequest ToRequest(this Customer customer) {
        return new CreateCustomerRequest {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Address = customer.Address,
            Phone = customer.Phone?.Value,
            Email = customer.Email?.Value,
        };
    }
}
