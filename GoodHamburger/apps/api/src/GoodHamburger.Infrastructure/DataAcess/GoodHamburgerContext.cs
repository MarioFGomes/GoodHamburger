using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess; 
public class GoodHamburgerContext: DbContext {
    public GoodHamburgerContext(DbContextOptions<GoodHamburgerContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoodHamburgerContext).Assembly);
    }
 }
