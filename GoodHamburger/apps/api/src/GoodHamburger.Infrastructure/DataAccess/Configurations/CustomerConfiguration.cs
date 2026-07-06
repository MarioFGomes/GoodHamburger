using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
              .HasMaxLength(50);

        builder.Property(s => s.Email)
              .HasMaxLength(200);

        builder.Property(s => s.Address)
             .HasMaxLength(200);

        builder.Property(o => o.CreatedAt)
                   .IsRequired();

        builder.Property(o => o.UpdatedAt)
               .IsRequired();

        // Uniqueness is checked in the use cases, but only the database can
        // guarantee it under concurrent requests.
        builder.HasIndex(s => s.Phone)
               .IsUnique()
               .HasFilter("[Phone] IS NOT NULL");
    }
}
