using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Menu;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Menu;
public class DeleteMenuUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var menu = MenuBuilder.Create().ToEntity();
        var repo = MenuRepositoryBuilder.Instance().WithMenu(menu).Build();
        var useCase = new DeleteMenuUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteMenuUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(menu.Id);
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task MenuNotFound() {
        var repo = MenuRepositoryBuilder.Instance().WithMenu(null).Build();
        var useCase = new DeleteMenuUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<DeleteMenuUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
