using FluentAssertions;
using GoodHamburger.Application.UseCases.Menu;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.Menu;
public class GetAllMenusUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var menus = MenuBuilder.CreateMany(3).Select(m => m.ToEntity()).ToList();
        var repo = MenuRepositoryBuilder.Instance()
            .WithMenus(menus)
            .WithCount(menus.Count)
            .Build();
        var useCase = new GetAllMenusUseCase(repo);
        var result = await useCase.ExecuteAsync(1, 10);
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task InvalidPageNormalized() {
        var repo = MenuRepositoryBuilder.Instance()
            .WithMenus(new List<GoodHamburger.Domain.Entities.Menu>())
            .WithCount(0)
            .Build();
        var useCase = new GetAllMenusUseCase(repo);
        var result = await useCase.ExecuteAsync(0, 0);
        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion
}
