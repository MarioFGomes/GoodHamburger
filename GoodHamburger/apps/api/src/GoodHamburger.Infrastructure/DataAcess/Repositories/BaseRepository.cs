using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase {
    
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger<BaseRepository<TEntity>> _logger;

    public BaseRepository(DbContext context, ILogger<BaseRepository<TEntity>> logger) {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _logger = logger;
    }

    public async Task AddOneAsync(TEntity entity) {
        await _dbSet.AddAsync(entity);
        try {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Entidade {typeof(TEntity).Name} salva com sucesso. Id: {entity.Id}");
        } catch (System.Exception ex) {
            _logger.LogError(ex, $"Erro ao salvar entidade {typeof(TEntity).Name}.");
            throw;
        }
    }

    public async Task AddManyAsync(List<TEntity> entity) {
        await _dbSet.AddRangeAsync(entity);
        try {
            await _context.SaveChangesAsync();
        } catch (System.Exception ex) {
            _logger.LogError(ex, $"Erro ao salvar entidade {typeof(TEntity).Name}.");
            throw;
        }
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> filterExpression) {

        var entities = await _dbSet.Where(filterExpression).ToListAsync();

        foreach (var entity in entities) {
            _dbSet.Remove(entity);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id) {
        
        var entity = await _dbSet.FindAsync(id);
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceOneAsync(Expression<Func<TEntity, bool>> filterExpression, TEntity entity) {
        var existingEntity = await _dbSet.FirstOrDefaultAsync(filterExpression);
        if (existingEntity != null) {
            existingEntity.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

        }
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceManyAsync(List<TEntity> entities, Func<TEntity, object> keySelector) {
        var keys = entities.Select(keySelector).ToList();
        var existingEntities = await _dbSet.Where(e => keys.Contains(keySelector(e))).ToListAsync();

        foreach (var entity in entities) {
            var key = keySelector(entity);
            var existing = existingEntities.FirstOrDefault(e => keySelector(e).Equals(key));

            if (existing != null) {
                _context.Entry(existing).CurrentValues.SetValues(entity);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filterExpression) {
        return await _dbSet.FirstOrDefaultAsync(filterExpression);
    }

    public async Task<TEntity> GetOneIncludingInactiveAsync(Expression<Func<TEntity, bool>> filterExpression) {
        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(filterExpression);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int page = 1, int pageSize = 10) {
        return await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync() {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllIncludingInactiveAsync() {
        return await _dbSet.IgnoreQueryFilters().ToListAsync();
    }

    public async Task<int> CountAsync() {
        return await _dbSet.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filterExpression) {
        return await _dbSet.CountAsync(filterExpression);
    }

    public IQueryable<TEntity> GetQueryable() {
        return _dbSet.AsQueryable();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filterExpression) {
        return await _dbSet.AnyAsync(filterExpression);
    }

    public void BeginTransaction() {
        if (_context.Database.CurrentTransaction is null)
            _context.Database.BeginTransaction();
    }

    public void CommitTransaction() {
        if (_context.Database.CurrentTransaction is not null)
            _context.Database.CommitTransaction();
    }

    public void RollbackTransaction() {
        if (_context.Database.CurrentTransaction is not null)
            _context.Database.RollbackTransaction();
    }
}