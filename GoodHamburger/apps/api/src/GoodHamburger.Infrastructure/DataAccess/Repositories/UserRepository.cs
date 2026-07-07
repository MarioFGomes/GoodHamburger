using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Infrastructure.DataAccess.Repositories;
public class UserRepository : BaseRepository<User>, IUserRepository {
    public UserRepository(GoodHamburgerContext context, ILogger<BaseRepository<User>> logger)
        : base(context, logger) { }
}
