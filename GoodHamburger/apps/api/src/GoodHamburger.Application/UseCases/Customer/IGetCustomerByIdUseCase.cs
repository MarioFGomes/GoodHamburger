using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.UseCases.Customer; 
public interface IGetCustomerByIdUseCase {
    Task<CustomerResponse> ExecuteAsync(Guid id, CancellationToken ct = default);
}
