using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Configurations;
public class OrderSideDishesConfiguration : IEntityTypeConfiguration<OrderSideDishes> {
    public void Configure(EntityTypeBuilder<OrderSideDishes> builder) {
        
        builder.ToTable("OrderSideDishes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderItemId).IsRequired();
        builder.Property(s => s.SideDishesId).IsRequired();
        builder.Property(s => s.Qtd).IsRequired();

        builder.Property(s => s.UnitPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(s => s.Category)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasIndex(s => s.SideDishesId);
    }
}
