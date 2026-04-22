using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.Exceptions; 
public class ResourceAlreadyExists:Exception {
public ResourceAlreadyExists(string resource, object id)
    : base($"{resource} Already Exists ") { } 
}
