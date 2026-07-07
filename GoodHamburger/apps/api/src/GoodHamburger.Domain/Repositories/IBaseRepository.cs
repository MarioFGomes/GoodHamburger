using System.Linq.Expressions;

namespace GoodHamburger.Domain.Repositories;
public interface IBaseRepository<TEntity> where TEntity : class {
    Task AddOneAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddManyAsync(List<TEntity> entity, CancellationToken cancellationToken);
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken);
    IQueryable<TEntity> GetQueryable();
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken);
}
