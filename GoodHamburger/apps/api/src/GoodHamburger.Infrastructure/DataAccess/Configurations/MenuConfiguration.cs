using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.DataAccess.Configurations;
public class MenuConfiguration : IEntityTypeConfiguration<Menu> {
    public void Configure(EntityTypeBuilder<Menu> builder) {

        builder.ToTable("Menus");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(m => m.Description)
               .HasMaxLength(500);

        // Money maps onto the same columns the flat decimal/enum used before,
        // so adopting the value object required no data migration.
        builder.OwnsOne(m => m.Price, price => {
            price.Property(p => p.Amount)
                 .HasColumnName("Price")
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

            price.Property(p => p.Currency)
                 .HasColumnName("Currency")
                 .HasConversion<string>()
                 .HasMaxLength(10)
                 .IsRequired();
        });
        builder.Navigation(m => m.Price).IsRequired();

        builder.Property(m => m.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();

        builder.HasIndex(m => m.Name)
               .IsUnique()
               .HasFilter("[IsDeleted] = 0");
    }
}
