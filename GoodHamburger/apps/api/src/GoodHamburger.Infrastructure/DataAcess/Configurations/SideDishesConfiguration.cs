using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Configurations;
public class SideDishesConfiguration : IEntityTypeConfiguration<SideDishes> {
    public void Configure(EntityTypeBuilder<SideDishes> builder) {
        
        builder.ToTable("SideDishes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(s => s.Description)
               .HasMaxLength(500);

        builder.Property(s => s.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(s => s.Category)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property<Currency>("Currency")
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();

        builder.Property<MenuStatus>("status")
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.CreatedAt)
                   .IsRequired();

        builder.Property(o => o.UpdatedAt)
               .IsRequired();
    }
}
