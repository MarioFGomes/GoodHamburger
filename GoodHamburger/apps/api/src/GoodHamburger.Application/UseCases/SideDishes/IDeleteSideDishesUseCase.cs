namespace GoodHamburger.Application.UseCases.SideDishes;
public interface IDeleteSideDishesUseCase {
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
