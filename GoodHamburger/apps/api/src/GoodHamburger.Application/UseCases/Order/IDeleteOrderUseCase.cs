namespace GoodHamburger.Application.UseCases.Order;
public interface IDeleteOrderUseCase {
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
