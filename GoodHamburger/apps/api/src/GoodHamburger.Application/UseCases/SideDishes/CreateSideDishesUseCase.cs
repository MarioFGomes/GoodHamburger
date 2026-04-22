using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Mappers;
using GoodHamburger.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.UseCases.SideDishes;
public class CreateSideDishesUseCase : ICreateSideDishesUseCase {

    private readonly ISideDishesRepository _sideDishRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSideDishesUseCase> _logger;

    public CreateSideDishesUseCase(ISideDishesRepository sideDishRepo, IUnitOfWork unitOfWork,
        ILogger<CreateSideDishesUseCase> logger) {
        _sideDishRepo = sideDishRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SideDishesResponse> ExecuteAsync(CreateSideDishesRequest request, CancellationToken ct = default) {

        var nameInUse = await _sideDishRepo.AnyAsync(s => s.Name == request.Name, ct);
        if (nameInUse) {
            _logger.LogWarning("Attempt to create a side dish with an existing name. Name={Name}", request.Name);
            throw new ResourceAlreadyExists("SideDish", request.Name);
        }

        var sideDish = request.ToDomain();

        await _sideDishRepo.AddOneAsync(sideDish, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("SideDish created. Id={SideDishId}, Name={Name}", sideDish.Id, sideDish.Name);

        return sideDish.ToResponse();
    }
}
