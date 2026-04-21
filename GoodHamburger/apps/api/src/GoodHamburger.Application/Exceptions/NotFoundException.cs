using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.Exceptions; 
public class NotFoundException:Exception {
    public NotFoundException(string resource, object id)
        : base($"{resource} com Id {id} não encontrado.") { }
}
