using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.SideDishes;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.SideDishes;
public class DeleteSideDishesUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var sideDish = SideDishesBuilder.Create().ToEntity();
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(sideDish).Build();
        var useCase = new DeleteSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteSideDishesUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(sideDish.Id);
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task SideDishNotFound() {
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(null).Build();
        var useCase = new DeleteSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteSideDishesUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
