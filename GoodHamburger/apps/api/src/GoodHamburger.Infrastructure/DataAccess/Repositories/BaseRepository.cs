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

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {

        var entities = await _dbSet.Where(filterExpression).ToListAsync(cancellationToken);

        foreach (var entity in entities) {
            _dbSet.Remove(entity);
        }

    }

    public async Task<bool> DeleteOneAsync(Guid id, CancellationToken cancellationToken = default) {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) {
            _logger.LogWarning("Entity {EntityType} with Id {Id} not found for deletion.",
                typeof(TEntity).Name, id);
            return false;
        }

        _dbSet.Remove(entity);
        return true;
    }

    public async Task<bool> ReplaceOneAsync(Expression<Func<TEntity, bool>> filterExpression, TEntity entity, CancellationToken cancellationToken = default) {

        var existingEntity = await _dbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);

        if (existingEntity is null) return false;

        // Identity and audit trail must survive a replace: the incoming entity
        // was rebuilt from a request DTO and carries a fresh Id/CreatedAt.
        var originalCreatedAt = existingEntity.CreatedAt;

        var entry = _context.Entry(existingEntity);
        entry.CurrentValues.SetValues(entity);

        existingEntity.CreatedAt = originalCreatedAt;
        existingEntity.UpdatedAt = DateTime.UtcNow;
        entry.Property(nameof(EntityBase.Id)).IsModified = false;

        return true;
    }

    public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync( CancellationToken cancellationToken = default, int page = 1, int pageSize = 10) {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return await _dbSet
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
