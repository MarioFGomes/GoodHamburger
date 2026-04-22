

using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Mappers; 
public static class CustomerMapper {

    public static CustomerResponse ToResponse(this Customer customer) {
       
        return new CustomerResponse { 
            Id = customer.Id,
            FirstName= customer.FirstName,
            LastName= customer.LastName,
            Address= customer.Address,
            Phone= customer.Phone,
            Email= customer.Email,
        };
    }

    public static Customer ToDomain(this CreateCustomerRequest CustomerRequest) {

        return new Customer {
            FirstName = CustomerRequest.FirstName,
            LastName = CustomerRequest.LastName,
            Address = CustomerRequest.Address,
            Phone = CustomerRequest.Phone,
            Email = CustomerRequest.Email,
        };
    }

    public static Customer ToDomain(this UpdateCustomerRequest CustomerRequest) {

        return new Customer {
            FirstName = CustomerRequest.FirstName,
            LastName = CustomerRequest.LastName,
            Phone = CustomerRequest.Phone,
            Email = CustomerRequest.Email,
            Address = CustomerRequest.Address,
        };
    }
}
