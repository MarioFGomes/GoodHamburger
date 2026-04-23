using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Menu;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Menu;
public class CreateMenuUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var request = MenuBuilder.Create().ToRequest();
        var useCase = BuildUseCase();
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NameAlreadyExists() {
        var request = MenuBuilder.Create().ToRequest();
        var menuRepo = MenuRepositoryBuilder.Instance().WithNameExists(true).Build();
        var useCase = new CreateMenuUseCase(menuRepo, UnitOfWorkBuilder.Instance().Build(), NullLogger<CreateMenuUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion

    private CreateMenuUseCase BuildUseCase() {
        return new CreateMenuUseCase(
            MenuRepositoryBuilder.Instance().Build(),
            UnitOfWorkBuilder.Instance().Build(),
            NullLogger<CreateMenuUseCase>.Instance);
    }
}
