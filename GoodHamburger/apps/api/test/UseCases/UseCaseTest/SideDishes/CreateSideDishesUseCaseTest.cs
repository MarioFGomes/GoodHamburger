using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.SideDishes;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.SideDishes;
public class CreateSideDishesUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var request = SideDishesBuilder.Create().ToRequest();
        var useCase = BuildUseCase();
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task NameAlreadyExists() {
        var request = SideDishesBuilder.Create().ToRequest();
        var repo = SideDishesRepositoryBuilder.Instance().WithNameExists(true).Build();
        var useCase = new CreateSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<CreateSideDishesUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion

    private CreateSideDishesUseCase BuildUseCase() {
        return new CreateSideDishesUseCase(
            SideDishesRepositoryBuilder.Instance().Build(),
            UnitOfWorkBuilder.Instance().Build(),
            NullLogger<CreateSideDishesUseCase>.Instance);
    }
}
