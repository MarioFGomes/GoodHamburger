using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.DataAccess.Configurations;
public class SideDishesConfiguration : IEntityTypeConfiguration<SideDishes> {
    public void Configure(EntityTypeBuilder<SideDishes> builder) {

        builder.ToTable("SideDishes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(s => s.Description)
               .HasMaxLength(500);

        builder.OwnsOne(s => s.Price, price => {
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
        builder.Navigation(s => s.Price).IsRequired();

        builder.Property(s => s.Category)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(s => s.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();

        builder.HasIndex(s => s.Name)
               .IsUnique()
               .HasFilter("[IsDeleted] = 0");
    }
}
