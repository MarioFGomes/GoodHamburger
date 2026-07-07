using Asp.Versioning;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Authorize]
public class OrdersController : EntityController {

    private readonly ICreateOrderUseCase _create;
    private readonly IGetOrderByIdUseCase _getById;
    private readonly IGetAllOrdersUseCase _getAll;
    private readonly IConfirmOrderUseCase _confirm;
    private readonly IPayOrderUseCase _pay;
    private readonly IMarkOrderReadyUseCase _ready;
    private readonly IDeliverOrderUseCase _deliver;
    private readonly ICancelOrderUseCase _cancel;
    private readonly IDeleteOrderUseCase _delete;

    public OrdersController(
        ICreateOrderUseCase create,
        IGetOrderByIdUseCase getById,
        IGetAllOrdersUseCase getAll,
        IConfirmOrderUseCase confirm,
        IPayOrderUseCase pay,
        IMarkOrderReadyUseCase ready,
        IDeliverOrderUseCase deliver,
        ICancelOrderUseCase cancel,
        IDeleteOrderUseCase delete) {
        _create = create;
        _getById = getById;
        _getAll = getAll;
        _confirm = confirm;
        _pay = pay;
        _ready = ready;
        _deliver = deliver;
        _cancel = cancel;
        _delete = delete;
    }

    /// <summary>Creates an order. Send an Idempotency-Key header to make retries safe.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken ct) {
        var response = await _create.ExecuteAsync(request, idempotencyKey, ct);
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

    [HttpPut("{id:guid}/pay")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Pay([FromRoute] Guid id, CancellationToken ct) {
        var response = await _pay.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response, "Order paid."));
    }

    /// <summary>Kitchen operation: only staff (ADMIN) can mark an order as ready.</summary>
    [HttpPut("{id:guid}/ready")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Ready([FromRoute] Guid id, CancellationToken ct) {
        var response = await _ready.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response, "Order ready."));
    }

    /// <summary>Kitchen operation: only staff (ADMIN) can mark an order as delivered.</summary>
    [HttpPut("{id:guid}/deliver")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Deliver([FromRoute] Guid id, CancellationToken ct) {
        var response = await _deliver.ExecuteAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.Ok(response, "Order delivered."));
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
        return Ok(ApiResponse.Ok("Order deleted."));
    }
}
