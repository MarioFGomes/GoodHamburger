using System.Text.Json;
using System.Text.Json.Serialization;
using MudBlazor.Services;
using WebGoodHamburger.Services;

namespace WebGoodHamburger {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices(config => {
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
            });

            builder.Services.AddSingleton(new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
            });

            var apiBaseUrl = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);

            builder.Services.AddSingleton<ApiTokenCache>();
            builder.Services.AddTransient<ApiAuthTokenHandler>();

            // Plain client used only to obtain tokens (no auth handler, no recursion).
            builder.Services.AddHttpClient("GoodHamburgerAuth", client => {
                client.BaseAddress = apiBaseUrl;
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            builder.Services.AddHttpClient("GoodHamburgerApi", client => {
                client.BaseAddress = apiBaseUrl;
                // Explicit timeout: a hung API must not pin Blazor circuits
                // for the 100-second default.
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ApiAuthTokenHandler>();

            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<MenuService>();
            builder.Services.AddScoped<SideDishService>();
            builder.Services.AddScoped<OrderService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
