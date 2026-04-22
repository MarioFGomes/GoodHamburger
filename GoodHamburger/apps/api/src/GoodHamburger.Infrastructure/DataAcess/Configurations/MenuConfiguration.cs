using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Configurations;
public class MenuConfiguration : IEntityTypeConfiguration<Menu> {
    public void Configure(EntityTypeBuilder<Menu> builder) {
        
        builder.ToTable("Menus");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(m => m.Description)
               .HasMaxLength(500);

        builder.Property(m => m.Price)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

      
        builder.Property(m => m.Currency)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(m => m.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.CreatedAt)
                   .IsRequired();

        builder.Property(o => o.UpdatedAt)
               .IsRequired();
    }
}
