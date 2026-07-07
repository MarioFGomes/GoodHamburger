using Asp.Versioning;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.SideDishes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/side-dishes")]
public class SideDishesController : EntityController {

    private readonly ICreateSideDishesUseCase _create;
    private readonly IGetSideDishByIdUseCase _getById;
    private readonly IGetAllSideDishesUseCase _getAll;
    private readonly IUpdateSideDishesUseCase _update;
    private readonly IDeleteSideDishesUseCase _delete;

    public SideDishesController(
        ICreateSideDishesUseCase create,
        IGetSideDishByIdUseCase getById,
        IGetAllSideDishesUseCase getAll,
        IUpdateSideDishesUseCase update,
        IDeleteSideDishesUseCase delete) {
        _create = create;
        _getById = getById;
        _getAll = getAll;
        _update = update;
        _delete = delete;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<SideDishesResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSideDishesRequest request, CancellationToken ct) {
        var response = await _create.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id },
            ApiResponse<SideDishesResponse>.Ok(response, "Side dish created.", StatusCodes.Status201Created));
    }

    [HttpGet("{id:guid}", Name = "SideDishes_GetById")]
    [ProducesResponseType(typeof(ApiResponse<SideDishesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) {
        var response = await _getById.ExecuteAsync(id, ct);
        return Ok(ApiResponse<SideDishesResponse>.Ok(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<SideDishesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) {
        var response = await _getAll.ExecuteAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResponse<SideDishesResponse>>.Ok(response));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<SideDishesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSideDishesRequest request, CancellationToken ct) {
        request.Id = id;
        var response = await _update.ExecuteAsync(request, ct);
        return Ok(ApiResponse<SideDishesResponse>.Ok(response, "Side dish updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) {
        await _delete.ExecuteAsync(id, ct);
        return Ok(ApiResponse.Ok("Side dish deleted."));
    }
}
