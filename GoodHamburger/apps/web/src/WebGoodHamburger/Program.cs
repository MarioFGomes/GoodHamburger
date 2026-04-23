using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebGoodHamburger.Services;

namespace WebGoodHamburger {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddSingleton(new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
            });

            builder.Services.AddHttpClient("GoodHamburgerApi", client => {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
            });

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
