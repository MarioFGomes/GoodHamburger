using FluentAssertions;
using GoodHamburger.Application.UseCases.SideDishes;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.SideDishes;
public class GetAllSideDishesUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var sideDishes = SideDishesBuilder.CreateMany(3).Select(s => s.ToEntity()).ToList();
        var repo = SideDishesRepositoryBuilder.Instance()
            .WithSideDishes(sideDishes)
            .WithCount(sideDishes.Count)
            .Build();
        var useCase = new GetAllSideDishesUseCase(repo);
        var result = await useCase.ExecuteAsync(1, 10);
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task InvalidPageNormalized() {
        var repo = SideDishesRepositoryBuilder.Instance()
            .WithSideDishes(new List<GoodHamburger.Domain.Entities.SideDishes>())
            .WithCount(0)
            .Build();
        var useCase = new GetAllSideDishesUseCase(repo);
        var result = await useCase.ExecuteAsync(0, 0);
        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion
}
