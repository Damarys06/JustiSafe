using JustiSafe.Core.Interfaces;
using JustiSafe.Core.Services;
using JustiSafe.Data;
using JustiSafe.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DI)
// ---------------------------------------------------------

// A. Configurar la conexión a SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<JustiSafeDbContext>(options =>
    options.UseSqlServer(connectionString));

// B. Registrar servicios de la capa de Negocio (Core)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICaseService, CaseService>();

// C. Configurar Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Si no estás logueado, te manda aquí
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/Account/Login";
    });

// D. Habilitar MVC (Controladores y Vistas)
// IMPORTANTE: Esto reemplaza o complementa a AddRazorPages para que funcionen los Controllers
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // <--- Activa el servicio

var app = builder.Build();

// ---------------------------------------------------------
// 2. CONFIGURACIÓN DEL PIPELINE HTTP (MIDDLEWARE)
// ---------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Orden Vital: Primero Autenticación (¿Quién eres?), luego Autorización (¿Qué puedes hacer?)
app.UseAuthentication();
app.UseAuthorization();

// E. Configurar el enrutamiento por defecto para MVC
// Esto le dice a la app: "Si no pido nada, ve al HomeController, acción Index"
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapHub<ChatHub>("/chathub");

app.Run();