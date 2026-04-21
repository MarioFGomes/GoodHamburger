using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Entities; 
public class OrderItem: EntityBase {
    public Guid OrderId { get; set; }
    public Guid MenuId { get; set; }
    public int Qtd { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; }

    private readonly List<OrderSideDishes> _OrderSideDishes = new();
    public IReadOnlyCollection<OrderSideDishes> OrderSideDishes => _OrderSideDishes;
}

