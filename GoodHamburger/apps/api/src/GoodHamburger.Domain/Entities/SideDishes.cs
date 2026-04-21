using GoodHamburger.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Entities; 
public class SideDishes: EntityBase {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public SideDishCategory Category { get; set; }
    private Currency Currency { get; set; } = Currency.BRL;
    private MenuStatus status { get; set; } = MenuStatus.available;
}
