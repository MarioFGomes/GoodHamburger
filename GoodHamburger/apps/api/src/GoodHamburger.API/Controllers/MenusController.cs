using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.Menu;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menus")]
public class MenusController : ControllerEntinty {

    private readonly ICreateMenuUseCase _create;
    private readonly IGetMenuByIdUseCase _getById;
    private readonly IGetAllMenusUseCase _getAll;
    private readonly IUpdateMenuUseCase _update;
    private readonly IDeleteMenuUseCase _delete;

    public MenusController(
        ICreateMenuUseCase create,
        IGetMenuByIdUseCase getById,
        IGetAllMenusUseCase getAll,
        IUpdateMenuUseCase update,
        IDeleteMenuUseCase delete) {
        _create = create;
        _getById = getById;
        _getAll = getAll;
        _update = update;
        _delete = delete;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateMenuRequest request, CancellationToken ct) {
       
        var response = await _create.ExecuteAsync(request, ct);
       
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}", Name = "Menus_GetById")]
    [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) {
        
        var response = await _getById.ExecuteAsync(id, ct);
        
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<MenuResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) {
        
        var response = await _getAll.ExecuteAsync(page, pageSize, ct);
        
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MenuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMenuRequest request, CancellationToken ct) {
        
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
