using GoodHamburger.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Entities; 
public class OrderItem: EntityBase {
    public Guid OrderId { get; set; }
    public Guid MenuId { get; set; }
    public int Qtd { get; private set; }
    public decimal UnitPrice { get; set; }
    public virtual Order Order { get; set; }

    private readonly List<OrderSideDishes> _OrderSideDishes = new();
    public IReadOnlyCollection<OrderSideDishes> OrderSideDishes => _OrderSideDishes;

    protected OrderItem() { }

    public OrderItem(Guid menuId, decimal unitPrice) {
        if (unitPrice < 0) throw new DomainException("Preço não pode ser negativo.");
        MenuId = menuId;
        Qtd = 1;                      
        UnitPrice = unitPrice;
    }

}

