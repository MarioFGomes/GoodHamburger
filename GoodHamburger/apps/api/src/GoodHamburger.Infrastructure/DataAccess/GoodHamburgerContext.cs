using System.Linq.Expressions;
using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAccess;
public class GoodHamburgerContext : DbContext {
    public GoodHamburgerContext(DbContextOptions<GoodHamburgerContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<int>("OrderNumbers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoodHamburgerContext).Assembly);
        ApplySoftDeleteFilters(modelBuilder);
    }

    /// <summary>
    /// Every EntityBase-derived table gets "WHERE IsDeleted = 0" appended to
    /// all queries automatically, so soft-deleted rows are invisible to the
    /// whole application without each repository having to remember it.
    /// </summary>
    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder) {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
            if (!typeof(EntityBase).IsAssignableFrom(entityType.ClrType) || entityType.IsOwned())
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var body = Expression.Not(Expression.Property(parameter, nameof(EntityBase.IsDeleted)));
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
        }
    }
}
