using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.DataAcess.Configurations {
    public class OrderConfiguration : IEntityTypeConfiguration<Order> {
        public void Configure(EntityTypeBuilder<Order> builder) {
            
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                   .IsRequired();

            builder.Property(o => o.CustomerID)
                   .IsRequired();
           
            builder.Property(o => o.Subtotal)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.Discount)
                   .HasColumnType("decimal(5,2)")   
                   .IsRequired();

            builder.Property(o => o.Total)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            
            builder.HasMany(o => o.OrderItems)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            
            builder.HasOne(o => o.Customer)
                   .WithMany()                     
                   .HasForeignKey(o => o.CustomerID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.CreatedAt)
                    .IsRequired();

            builder.Property(o => o.UpdatedAt)
                   .IsRequired();

            builder.Metadata
                   .FindNavigation(nameof(Order.OrderItems))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(o => o.OrderNumber).IsUnique();
        }
    }
}
