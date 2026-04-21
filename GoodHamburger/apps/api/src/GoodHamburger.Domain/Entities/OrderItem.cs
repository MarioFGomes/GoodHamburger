using GoodHamburger.Domain.Enum;
using GoodHamburger.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Domain.Entities; 
public class OrderItem {
    public Guid Id { get; set; }
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
        Id = Guid.NewGuid();
    }

    public void AddSideDish(Guid sideDishesId, SideDishCategory category, decimal unitPrice) {
      
        var alreadyHasCategory = _OrderSideDishes.Any(s => s.Category == category);
        
        if (alreadyHasCategory) {
            var nome = category == SideDishCategory.FRIES ? "batata frita" : "refrigerante";
            throw new DomainException(
                $"Este pedido já contém um {nome}. Não é permitido duplicar acompanhamentos.");
        }

        _OrderSideDishes.Add(new OrderSideDishes(sideDishesId, category, unitPrice));
    }

    public bool HasFries() => _OrderSideDishes.Any(s => s.Category == SideDishCategory.FRIES);
    public bool HasDrink() => _OrderSideDishes.Any(s => s.Category == SideDishCategory.DRINK);

    public decimal CalculateTotal() {
        var sideDishesTotal = _OrderSideDishes.Sum(sd => sd.CalculateTotal());
        return (UnitPrice * Qtd) + sideDishesTotal;
    }

}

