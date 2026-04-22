using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class SideDishesRepository: BaseRepository<SideDishes>, ISideDishesRepository {
    public SideDishesRepository(GoodHamburgerContext _context, ILogger<BaseRepository<SideDishes>> logger) : base(_context, logger) { }
}
