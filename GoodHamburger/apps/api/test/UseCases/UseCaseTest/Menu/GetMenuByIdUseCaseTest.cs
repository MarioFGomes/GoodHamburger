using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.Menu;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Menu;
public class GetMenuByIdUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var menu = MenuBuilder.Create().ToEntity();
        var repo = MenuRepositoryBuilder.Instance().WithMenu(menu).Build();
        var useCase = new GetMenuByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(menu.Id);
        result.Should().NotBeNull();
        result.Id.Should().Be(menu.Id);
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = MenuRepositoryBuilder.Instance().WithMenu(null).Build();
        var useCase = new GetMenuByIdUseCase(repo);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
