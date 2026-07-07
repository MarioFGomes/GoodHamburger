using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using GoodHamburger.Application.DTOs.Requests;
using GoodHamburger.Application.DTOs.Responses;
using GoodHamburger.Domain.Enum;

namespace IntegrationTest;

public class OrderFlowIntegrationTest : IClassFixture<GoodHamburgerApiFactory>, IAsyncLifetime {

    private readonly GoodHamburgerApiFactory _factory;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
        Converters = { new JsonStringEnumConverter() }
    };

    public OrderFlowIntegrationTest(GoodHamburgerApiFactory factory) {
        _factory = factory;
    }

    public async Task InitializeAsync() {
        _client = await _factory.CreateAdminClientAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<CustomerResponse> CreateCustomerAsync() {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var response = await _client.PostAsJsonAsync("/api/v1/customers", new CreateCustomerRequest {
            FirstName = "Maria",
            LastName = "Souza",
            Phone = $"+55{Random.Shared.NextInt64(100_000_000, 999_999_999)}",
            Email = $"maria.{unique}@test.com",
            Address = "Luanda"
        }, JsonOptions);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<CustomerResponse>>(JsonOptions))!;
        envelope.Success.Should().BeTrue();
        return envelope.Data!;
    }

    private async Task<MenuResponse> CreateMenuAsync(decimal price = 5m) {
        var response = await _client.PostAsJsonAsync("/api/v1/menus", new CreateMenuRequest {
            Name = $"X Burger {Guid.NewGuid():N}",
            Description = "Test burger",
            Price = price
        }, JsonOptions);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<MenuResponse>>(JsonOptions))!;
        envelope.Success.Should().BeTrue();
        return envelope.Data!;
    }

    private async Task<SideDishesResponse> CreateSideDishAsync(SideDishCategory category, decimal price) {
        var response = await _client.PostAsJsonAsync("/api/v1/side-dishes", new CreateSideDishesRequest {
            Name = $"{category} {Guid.NewGuid():N}",
            Description = "Test side dish",
            Price = price,
            Category = category
        }, JsonOptions);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<SideDishesResponse>>(JsonOptions))!;
        envelope.Success.Should().BeTrue();
        return envelope.Data!;
    }

    private async Task<OrderResponse> CreateOrderAsync(Guid customerId, Guid menuId, string? idempotencyKey = null) {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders") {
            Content = JsonContent.Create(new CreateOrderRequest { CustomerId = customerId, MenuId = menuId }, options: JsonOptions)
        };
        if (idempotencyKey is not null)
            request.Headers.Add("Idempotency-Key", idempotencyKey);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>(JsonOptions))!.Data!;
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy() {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FullOrderFlow_CreateConfirm_AppliesComboDiscount() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync(price: 5m);
        var fries = await CreateSideDishAsync(SideDishCategory.FRIES, 2m);
        var drink = await CreateSideDishAsync(SideDishCategory.DRINK, 2.5m);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/orders", new CreateOrderRequest {
            CustomerId = customer.Id,
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { fries.Id, drink.Id }
        }, JsonOptions);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var envelope = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>(JsonOptions))!;
        envelope.Success.Should().BeTrue();
        envelope.StatusCode.Should().Be(201);
        var order = envelope.Data!;

        order.Subtotal.Should().Be(9.5m);
        order.DiscountPercentage.Should().Be(20m);
        order.Total.Should().Be(7.6m);
        order.Status.Should().Be(OrderStatus.PENDING);

        var confirmResponse = await _client.PutAsync($"/api/v1/orders/{order.Id}/confirm", null);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var confirmed = (await confirmResponse.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>(JsonOptions))!;
        confirmed.Success.Should().BeTrue();
        confirmed.Data!.Status.Should().Be(OrderStatus.CONFIRMED);
    }

    [Fact]
    public async Task StateMachine_FullLifecycle_ConfirmPayReadyDeliver() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        var order = await CreateOrderAsync(customer.Id, menu.Id);

        foreach (var (action, expected) in new[] {
            ("confirm", OrderStatus.CONFIRMED),
            ("pay", OrderStatus.PAID),
            ("ready", OrderStatus.READY),
            ("deliver", OrderStatus.DELIVERED),
        }) {
            var response = await _client.PutAsync($"/api/v1/orders/{order.Id}/{action}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK, $"'{action}' should be a valid transition");
            var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>(JsonOptions))!;
            envelope.Data!.Status.Should().Be(expected);
        }
    }

    [Fact]
    public async Task StateMachine_PayBeforeConfirm_Returns422() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        var order = await CreateOrderAsync(customer.Id, menu.Id);

        var response = await _client.PutAsync($"/api/v1/orders/{order.Id}/pay", null);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Message.Should().Contain("transition");
        envelope.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task OrderCreation_WithIdempotencyKey_IsReplaySafe() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        var key = Guid.NewGuid().ToString("N");

        var first = await CreateOrderAsync(customer.Id, menu.Id, key);
        var replay = await CreateOrderAsync(customer.Id, menu.Id, key);

        replay.Id.Should().Be(first.Id);
        replay.OrderNumber.Should().Be(first.OrderNumber);
    }

    [Fact]
    public async Task ConfirmedOrder_CannotBeDeleted() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        var order = await CreateOrderAsync(customer.Id, menu.Id);

        await _client.PutAsync($"/api/v1/orders/{order.Id}/confirm", null);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/orders/{order.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var envelope = (await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CustomerWithOrders_CannotBeDeleted() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        await CreateOrderAsync(customer.Id, menu.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/customers/{customer.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task DuplicatedCustomerPhone_Returns409() {
        var customer = await CreateCustomerAsync();

        var duplicate = await _client.PostAsJsonAsync("/api/v1/customers", new CreateCustomerRequest {
            FirstName = "Outro",
            LastName = "Cliente",
            Phone = customer.Phone,
            Email = $"outro.{Guid.NewGuid():N}@test.com",
            Address = "Benguela"
        }, JsonOptions);

        duplicate.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var envelope = (await duplicate.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.StatusCode.Should().Be(409);
        envelope.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UnavailableMenu_CannotBeOrdered() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/menus/{menu.Id}", new UpdateMenuRequest {
            Name = menu.Name,
            Description = menu.Description,
            Price = menu.Price,
            Status = MenuStatus.Unavailable
        }, JsonOptions);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderResponse = await _client.PostAsJsonAsync("/api/v1/orders", new CreateOrderRequest {
            CustomerId = customer.Id,
            MenuId = menu.Id
        }, JsonOptions);

        orderResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task OrderWithTwoDrinks_IsRejected() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();
        var drink1 = await CreateSideDishAsync(SideDishCategory.DRINK, 2.5m);
        var drink2 = await CreateSideDishAsync(SideDishCategory.DRINK, 3m);

        var response = await _client.PostAsJsonAsync("/api/v1/orders", new CreateOrderRequest {
            CustomerId = customer.Id,
            MenuId = menu.Id,
            SideDishIds = new List<Guid> { drink1.Id, drink2.Id }
        }, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task OrderWithNullSideDishIds_Returns400Envelope_Not500() {
        var customer = await CreateCustomerAsync();
        var menu = await CreateMenuAsync();

        // Explicit null used to NRE inside the validator (500). On .NET 10 the
        // serializer rejects null for the non-nullable list; the contract must
        // still answer with the standard envelope, not a bare ProblemDetails.
        var payload = $$"""{"customerId":"{{customer.Id}}","menuId":"{{menu.Id}}","sideDishIds":null}""";
        var response = await _client.PostAsync("/api/v1/orders",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.Errors.Should().NotBeNullOrEmpty();
        envelope.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetMissingOrder_Returns404() {
        var response = await _client.GetAsync($"/api/v1/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.StatusCode.Should().Be(404);
        envelope.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvalidCustomerPayload_Returns400WithValidationErrors() {
        var response = await _client.PostAsJsonAsync("/api/v1/customers", new CreateCustomerRequest {
            FirstName = "",
            LastName = "",
            Phone = "abc",
            Email = "not-an-email"
        }, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateCustomer_DoesNotChangeCreatedAt() {
        var customer = await CreateCustomerAsync();

        var before = (await (await _client.GetAsync($"/api/v1/customers/{customer.Id}"))
            .Content.ReadFromJsonAsync<ApiResponse<CustomerResponse>>(JsonOptions))!.Data!;

        var update = await _client.PutAsJsonAsync($"/api/v1/customers/{customer.Id}", new UpdateCustomerRequest {
            FirstName = "Maria Editada",
            LastName = "Souza",
            Phone = customer.Phone,
            Email = customer.Email,
            Address = "Huambo"
        }, JsonOptions);
        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = (await (await _client.GetAsync($"/api/v1/customers/{customer.Id}"))
            .Content.ReadFromJsonAsync<ApiResponse<CustomerResponse>>(JsonOptions))!.Data!;

        updated.FirstName.Should().Be("Maria Editada");
        updated.CreatedAt.Should().Be(before.CreatedAt, "updates must never rewrite the audit timestamp");
        updated.UpdatedAt.Should().BeOnOrAfter(before.UpdatedAt);
    }

    [Fact]
    public async Task DeletedMenu_IsSoftDeleted_AndHiddenFromQueries() {
        var menu = await CreateMenuAsync();

        var response = await _client.DeleteAsync($"/api/v1/menus/{menu.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeTrue();
        envelope.Message.Should().Be("Menu deleted.");

        // The global query filter must hide the soft-deleted row everywhere.
        var getAfter = await _client.GetAsync($"/api/v1/menus/{menu.Id}");
        getAfter.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Requests_WithoutToken_Get401Envelope() {
        var anonymous = _factory.CreateClient();

        var response = await anonymous.GetAsync($"/api/v1/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var envelope = (await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.StatusCode.Should().Be(401);
        envelope.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegularUser_CannotManageCatalog_Gets403() {
        var anonymous = _factory.CreateClient();

        var register = await anonymous.PostAsJsonAsync("/api/v1/auth/register", new RegisterUserRequest {
            Name = "Regular User",
            Email = $"user.{Guid.NewGuid():N}@test.com",
            Password = "UserPass123"
        }, JsonOptions);
        register.StatusCode.Should().Be(HttpStatusCode.Created);
        var auth = (await register.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptions))!.Data!;
        auth.Role.Should().Be("USER");

        var userClient = _factory.CreateClient();
        userClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Reads are public…
        var list = await userClient.GetAsync("/api/v1/menus");
        list.StatusCode.Should().Be(HttpStatusCode.OK);

        // …catalog writes are ADMIN only.
        var create = await userClient.PostAsJsonAsync("/api/v1/menus", new CreateMenuRequest {
            Name = $"Hacker Burger {Guid.NewGuid():N}",
            Price = 1m
        }, JsonOptions);
        create.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var envelope = (await create.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions))!;
        envelope.Success.Should().BeFalse();
        envelope.StatusCode.Should().Be(403);
    }
}
