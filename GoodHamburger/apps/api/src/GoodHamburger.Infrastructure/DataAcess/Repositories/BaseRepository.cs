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

    public async Task AddOneAsync(TEntity entity, CancellationToken cancellationToken = default) {
        
        try {
            await _dbSet.AddAsync(entity, cancellationToken);
            _logger.LogInformation($"Entidade {typeof(TEntity).Name} salva com sucesso. Id: {entity.Id}");
        } catch (System.Exception ex) {
            _logger.LogError(ex, $"Erro ao salvar entidade {typeof(TEntity).Name}.");
            throw;
        }
    }

    public async Task AddManyAsync(List<TEntity> entity, CancellationToken cancellationToken = default) {
       
        try {
            await _dbSet.AddRangeAsync(entity, cancellationToken);
        } catch (System.Exception ex) {
            _logger.LogError(ex, $"Erro ao salvar entidade {typeof(TEntity).Name}.");
            throw;
        }
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {

        var entities = await _dbSet.Where(filterExpression).ToListAsync(cancellationToken);

        foreach (var entity in entities) {
            _dbSet.Remove(entity);
        }
       
    }

    public async Task<bool> DeleteOneAsync(Guid id, CancellationToken cancellationToken = default) {
        var entity = await _dbSet.FindAsync(id, cancellationToken);
        if (entity == null) {
            _logger.LogWarning("Entidade {EntityType} com Id {Id} não encontrada para deletar.",
                typeof(TEntity).Name, id);
            return false;
        }

        _dbSet.Remove(entity);
        return true;
    }

        public async Task<bool> ReplaceOneAsync(Expression<Func<TEntity, bool>> filterExpression, TEntity entity, CancellationToken cancellationToken = default) {
        
        var existingEntity = await _dbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);
       
        if (existingEntity is null) return false;
            
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            existingEntity.UpdatedAt = DateTime.UtcNow;
             _context.Entry(existingEntity).Property(nameof(EntityBase.Id)).IsModified = false;
          return true;
    }

    public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public async Task<TEntity?> GetOneIncludingInactiveAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) {
        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync( CancellationToken cancellationToken = default, int page = 1, int pageSize = 10) {
        return await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllIncludingInactiveAsync(CancellationToken cancellationToken = default) {
        return await _dbSet.IgnoreQueryFilters().ToListAsync(cancellationToken);
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