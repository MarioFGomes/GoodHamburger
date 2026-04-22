using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;

namespace GoodHamburger.Application.UseCases.SideDishes;
public class GetSideDishByIdUseCase : IGetSideDishByIdUseCase {

    private readonly ISideDishesRepository _sideDishRepo;

    public GetSideDishByIdUseCase(ISideDishesRepository sideDishRepo) => _sideDishRepo = sideDishRepo;

    public async Task<SideDishesResponse> ExecuteAsync(Guid id, CancellationToken ct = default) {
        var sideDish = await _sideDishRepo.GetOneAsync(s => s.Id == id, ct);
        if (sideDish is null) throw new NotFoundException("SideDish", id);
        return sideDish.ToResponse();
    }
}
