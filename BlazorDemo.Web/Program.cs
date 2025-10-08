using BlazorDemo.Web.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Allow CORS for local dev (later will switch to WASM)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHttpClient();

var app = builder.Build();

// HTTP request pipeline setup.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); //  HTTP Strict Transport Security — it’s a web security feature that helps enforce HTTPS connections and prevent users from being downgraded to HTTP.
    // Example header: Strict-Transport-Security: max-age=31536000; includeSubDomains
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseCors();

app.MapGet("/api/weather", () =>
{
    var rng = new Random();
    var summaries = new[]
    {
        "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
    };

    var data = Enumerable.Range(1, 5).Select(i =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
            rng.Next(-5, 35),
            summaries[rng.Next(summaries.Length)]
        )
    );

    return Results.Ok(data);
});

// Static assets + Razor components
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}