using FluentAssertions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.UseCases.SideDishes;
using Microsoft.Extensions.Logging.Abstractions;
using Utils.Entities;
using Utils.Repositories;

namespace UseCaseTest.SideDishes;
public class UpdateSideDishesUseCaseTest {

    #region Sucess

    [Fact]
    public async Task Success() {
        var existingSideDish = SideDishesBuilder.Create().ToEntity();
        var request = SideDishesBuilder.Create().ToUpdateRequest();
        request.Id = existingSideDish.Id;
        request.Name = existingSideDish.Name;
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(existingSideDish).Build();
        var useCase = new UpdateSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateSideDishesUseCase>.Instance);
        var result = await useCase.ExecuteAsync(request);
        result.Should().NotBeNull();
    }

    #endregion

    #region Fail

    [Fact]
    public async Task SideDishNotFound() {
        var request = SideDishesBuilder.Create().ToUpdateRequest();
        var repo = SideDishesRepositoryBuilder.Instance().WithSideDish(null).Build();
        var useCase = new UpdateSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateSideDishesUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task NameAlreadyExists() {
        var existingSideDish = SideDishesBuilder.Create().ToEntity();
        var request = SideDishesBuilder.Create().ToUpdateRequest();
        request.Id = existingSideDish.Id;
        var repo = SideDishesRepositoryBuilder.Instance()
            .WithSideDish(existingSideDish)
            .WithNameExists(true)
            .Build();
        var useCase = new UpdateSideDishesUseCase(repo, UnitOfWorkBuilder.Instance().Build(), NullLogger<UpdateSideDishesUseCase>.Instance);
        var act = () => useCase.ExecuteAsync(request);
        await act.Should().ThrowAsync<ResourceAlreadyExists>();
    }

    #endregion
}
