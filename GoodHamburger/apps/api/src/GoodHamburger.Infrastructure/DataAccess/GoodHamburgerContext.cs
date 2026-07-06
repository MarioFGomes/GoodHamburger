using GoodHamburger.Infrastructure.DataAccess.Seeds;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAccess;
public class GoodHamburgerContext: DbContext {
    public GoodHamburgerContext(DbContextOptions<GoodHamburgerContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<int>("OrderNumbers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoodHamburgerContext).Assembly);
        SeedData.Seed(modelBuilder);
    }
 }
