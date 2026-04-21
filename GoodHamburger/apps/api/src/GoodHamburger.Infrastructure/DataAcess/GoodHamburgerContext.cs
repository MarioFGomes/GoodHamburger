using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess; 
public class GoodHamburgerContext: DbContext {
    public GoodHamburgerContext(DbContextOptions<GoodHamburgerContext> options) : base(options) {
        Database.EnsureCreated();  
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoodHamburgerContext).Assembly);
    }
 }
