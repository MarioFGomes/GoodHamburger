using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Configurations;
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem> {
    public void Configure(EntityTypeBuilder<OrderItem> builder) {
       
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.OrderId).IsRequired();
        builder.Property(i => i.MenuId).IsRequired();

        builder.Property(i => i.Qtd).IsRequired();

        builder.Property(i => i.UnitPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.HasMany(i => i.OrderSideDishes)
               .WithOne(s => s.OrderItem)
               .HasForeignKey(s => s.OrderItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Menu)
                  .WithMany()
                  .HasForeignKey(o => o.MenuId)
                  .OnDelete(DeleteBehavior.Restrict);


        builder.Metadata
               .FindNavigation(nameof(OrderItem.OrderSideDishes))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
