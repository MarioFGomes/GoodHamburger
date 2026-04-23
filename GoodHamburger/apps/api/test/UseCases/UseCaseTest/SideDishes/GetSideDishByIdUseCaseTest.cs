using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.SideDishes;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.SideDishes;
public class GetSideDishByIdUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var sideDish = SideDishesBuilder.Create().ToEntity();
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(sideDish).Build();
        var useCase = new GetSideDishByIdUseCase(repo);
        var result = await useCase.ExecuteAsync(sideDish.Id);
        result.Should().NotBeNull();
        result.Id.Should().Be(sideDish.Id);
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NotFound() {
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(null).Build();
        var useCase = new GetSideDishByIdUseCase(repo);
        var act = () => useCase.ExecuteAsync(Guid.NewGuid());
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}
