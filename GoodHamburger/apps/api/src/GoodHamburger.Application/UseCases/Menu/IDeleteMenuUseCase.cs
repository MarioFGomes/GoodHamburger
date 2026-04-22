namespace GoodHamburger.Application.UseCases.Menu;
public interface IDeleteMenuUseCase {
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
