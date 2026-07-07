using Asp.Versioning;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Application.UseCases.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
[Authorize]
public class CustomersController : EntityController {

    private readonly ICreateCustomerUseCase _createCustomer;
    private readonly IGetCustomerByIdUseCase _getById;
    private readonly IGetAllCustomersUseCase _getAll;
    private readonly IUpdateCustomerUseCase _update;
    private readonly IDeleteCustomerUseCase _delete;

    public CustomersController(
        ICreateCustomerUseCase createCustomer,
        IGetCustomerByIdUseCase getById,
        IGetAllCustomersUseCase getAll,
        IUpdateCustomerUseCase update,
        IDeleteCustomerUseCase delete) {
        _createCustomer = createCustomer;
        _getById = getById;
        _getAll = getAll;
        _update = update;
        _delete = delete;
    }


    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request,CancellationToken ct) {

        var response = await _createCustomer.ExecuteAsync(request, ct);

        return CreatedAtAction(
               nameof(GetById),
               new { id = response.Id },
               ApiResponse<CustomerResponse>.Ok(response, "Customer created.", StatusCodes.Status201Created));
    }


    [HttpGet("{id:guid}", Name = "Customers_GetById")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id,CancellationToken ct) {

        var response = await _getById.ExecuteAsync(id, ct);

        return Ok(ApiResponse<CustomerResponse>.Ok(response));
    }


    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<CustomerResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1,[FromQuery] int pageSize = 10,CancellationToken ct = default) {

        var response = await _getAll.ExecuteAsync(page, pageSize, ct);

        return Ok(ApiResponse<PagedResponse<CustomerResponse>>.Ok(response));
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateCustomerRequest request,CancellationToken ct) {

        request.Id = id;

        var response = await _update.ExecuteAsync(request, ct);

        return Ok(ApiResponse<CustomerResponse>.Ok(response, "Customer updated."));
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,CancellationToken ct) {

        await _delete.ExecuteAsync(id, ct);

        return Ok(ApiResponse.Ok("Customer deleted."));
    }
}
