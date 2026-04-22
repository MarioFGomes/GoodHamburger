using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.SideDishes;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/side-dishes")]
public class SideDishesController : ControllerEntinty {

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
    [ProducesResponseType(typeof(SideDishesResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSideDishesRequest request, CancellationToken ct) {
        var response = await _create.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(SideDishesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) {
        var response = await _getById.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SideDishesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) {
        var response = await _getAll.ExecuteAsync(page, pageSize, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SideDishesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSideDishesRequest request, CancellationToken ct) {
        request.Id = id;
        var response = await _update.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) {
        await _delete.ExecuteAsync(id, ct);
        return NoContent();
    }
}
