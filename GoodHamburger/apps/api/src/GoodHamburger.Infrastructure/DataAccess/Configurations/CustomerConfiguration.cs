using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.DataAccess.Configurations;
public class CustomerConfiguration : IEntityTypeConfiguration<Customer> {
    public void Configure(EntityTypeBuilder<Customer> builder) {

        builder.ToTable("Customer");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FirstName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(s => s.LastName)
               .HasMaxLength(200);

        builder.Property(s => s.Phone)
               .HasConversion(p => p.Value, v => Phone.Create(v))
               .HasMaxLength(50);

        builder.Property(s => s.Email)
               .HasConversion(e => e.Value, v => Email.Create(v))
               .HasMaxLength(200);

        builder.Property(s => s.Address)
               .HasMaxLength(200);

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();

        // Uniqueness is checked in the use cases, but only the database can
        // guarantee it under concurrent requests. Soft-deleted rows must not
        // block reuse of the phone, hence the filtered index.
        builder.HasIndex(s => s.Phone)
               .IsUnique()
               .HasFilter("[Phone] IS NOT NULL AND [IsDeleted] = 0");
    }
}
