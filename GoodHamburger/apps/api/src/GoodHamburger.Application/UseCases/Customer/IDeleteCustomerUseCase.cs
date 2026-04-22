using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.UseCases.Customer; 
public interface IDeleteCustomerUseCase {
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
