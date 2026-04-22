using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.SideDishes;
public class UpdateSideDishesUseCase : IUpdateSideDishesUseCase {

    private readonly ISideDishesRepository _sideDishRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSideDishesUseCase> _logger;

    public UpdateSideDishesUseCase(ISideDishesRepository sideDishRepo, IUnitOfWork unitOfWork,
        ILogger<UpdateSideDishesUseCase> logger) {
        _sideDishRepo = sideDishRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SideDishesResponse> ExecuteAsync(UpdateSideDishesRequest request, CancellationToken ct = default) {

        var sideDish = await _sideDishRepo.GetOneAsync(s => s.Id == request.Id, ct)
            ?? throw new NotFoundException("SideDish", request.Id);

        if (sideDish.Name != request.Name) {
            var nameInUse = await _sideDishRepo.AnyAsync(
                s => s.Name == request.Name && s.Id != request.Id, ct);
            if (nameInUse)
                throw new ResourceAlreadyExists("SideDish", request.Name);
        }

        var sideDishToUpdate = request.ToDomain();

        await _sideDishRepo.ReplaceOneAsync(s => s.Id == sideDish.Id, sideDishToUpdate, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("SideDish updated. Id={SideDishId}", sideDish.Id);

        return sideDishToUpdate.ToResponse();
    }
}
