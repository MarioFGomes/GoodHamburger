using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class MenuRepository: BaseRepository<Menu>, IMenuRepository {
    public MenuRepository(GoodHamburgerContext _context, ILogger<BaseRepository<Menu>> logger) : base(_context, logger) { }
}
