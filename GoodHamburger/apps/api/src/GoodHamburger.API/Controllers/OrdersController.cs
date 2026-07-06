using Asp.Versioning;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.Order;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : EntityController {

    private readonly ICreateOrderUseCase _create;
    private readonly IGetOrderByIdUseCase _getById;
    private readonly IGetAllOrdersUseCase _getAll;
    private readonly IConfirmOrderUseCase _confirm;
    private readonly ICancelOrderUseCase _cancel;
    private readonly IDeleteOrderUseCase _delete;

    public OrdersController(
        ICreateOrderUseCase create,
        IGetOrderByIdUseCase getById,
        IGetAllOrdersUseCase getAll,
        IConfirmOrderUseCase confirm,
        ICancelOrderUseCase cancel,
        IDeleteOrderUseCase delete) {
        _create = create;
        _getById = getById;
        _getAll = getAll;
        _confirm = confirm;
        _cancel = cancel;
        _delete = delete;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct) {
        var response = await _create.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id },
            ApiResponse<OrderResponse>.Ok(response, "Order created.", StatusCodes.Status201Created));
    }

    [HttpGet("{id:guid}", Name = "Orders_GetById")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) {
        var response = await _getById.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) {
        var response = await _getAll.ExecuteAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResponse<OrderResponse>>.Ok(response));
    }

    [HttpPut("{id:guid}/confirm")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Confirm([FromRoute] Guid id, CancellationToken ct) {
        var response = await _confirm.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response, "Order confirmed."));
    }

    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken ct) {
        var response = await _cancel.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response, "Order cancelled."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) {
        await _delete.ExecuteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Order deleted."));
    }
}
