using GoodHamburger.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.DataAcess.Repositories; 
public class UnitOfWork: IUnitOfWork {
   
    private readonly DbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(DbContext context) => _context = context;

    private bool SupportsTransactions => _context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory";

    public async Task BeginTransactionAsync(CancellationToken ct = default) {
        if (SupportsTransactions)
            _transaction ??= await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default) {
        await _context.SaveChangesAsync(ct);
        if (_transaction is not null) {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default) {
        if (_transaction is not null) {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async ValueTask DisposeAsync() {
        if (_transaction is not null) await _transaction.DisposeAsync();
    }
}
