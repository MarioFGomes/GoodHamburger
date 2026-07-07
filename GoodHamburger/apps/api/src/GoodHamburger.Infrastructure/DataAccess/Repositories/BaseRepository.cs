using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAccess.Repositories;
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase {

    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger<BaseRepository<TEntity>> _logger;

    public BaseRepository(DbContext context, ILogger<BaseRepository<TEntity>> logger) {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _logger = logger;
    }

    public async Task AddOneAsync(TEntity entity, CancellationToken cancellationToken = default) {
        await _dbSet.AddAsync(entity, cancellationToken);
        _logger.LogDebug("Entity {EntityType} tracked for insert. Id={Id}", typeof(TEntity).Name, entity.Id);
    }

    public async Task AddManyAsync(List<TEntity> entity, CancellationToken cancellationToken = default) {
        await _dbSet.AddRangeAsync(entity, cancellationToken);
    }

    public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default) {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return await _dbSet
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.CountAsync(filterExpression, cancellationToken);
    }

    public IQueryable<TEntity> GetQueryable() {
        return _dbSet.AsQueryable();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.AnyAsync(filterExpression, cancellationToken);
    }
}
