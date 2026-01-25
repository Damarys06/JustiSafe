using JustiSafe.Core.Interfaces;
using JustiSafe.Core.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using JustiSafe.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE SERVICIOS
// 1. CONFIGURACIÓN DE SERVICIOS
// Eliminamos DbContext directo
// builder.Services.AddDbContext<JustiSafeDbContext>(...);

// Registramos Cliente HTTP para el Gateway
builder.Services.AddHttpClient("GatewayClient", client =>
{
    // Use configuration value (in Docker it is http://gateway:8080)
    var gatewayUrl = builder.Configuration["GatewayUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(gatewayUrl);
});

// Removemos servicios internos antiguos
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<ICaseService, CaseService>();

// CONFIGURACIÓN DEL TIMEOUT DE SESIÓN
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";

        // CAMBIO: La sesión muere tras 40 segundos de inactividad
        options.ExpireTimeSpan = TimeSpan.FromSeconds(40);

        // CAMBIO: Si el usuario hace clic, el contador se reinicia (True)
        // Si lo pones en False, se cerrará a los 40s aunque esté trabajando.
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// 2. PIPELINE HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapHub<JustiSafe.Web.Hubs.ChatHub>("/chathub");

app.Run();