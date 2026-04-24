using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Menu;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Menu;
public class UpdateMenuUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var existingMenu = MenuBuilder.Create().ToEntity();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Id = existingMenu.Id;
        request.Name = existingMenu.Name;
        var repo = MenuRepositoryBuilder.Instance().WithMenu(existingMenu).Build();
        var useCase = new UpdateMenuUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateMenuUseCase>.Instance);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task MenuNotFound() {
        var request = MenuBuilder.Create().ToUpdateRequest();
        var repo = MenuRepositoryBuilder.Instance().WithMenu(null).Build();
        var useCase = new UpdateMenuUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateMenuUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task NameAlreadyExists() {
        var existingMenu = MenuBuilder.Create().ToEntity();
        var request = MenuBuilder.Create().ToUpdateRequest();
        request.Id = existingMenu.Id;
        var repo = MenuRepositoryBuilder.Instance()
            .WithMenu(existingMenu)
            .WithNameExists(true)
            .Build();
        var useCase = new UpdateMenuUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateMenuUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion
}
