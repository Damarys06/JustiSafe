using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using JustiSafe.Core.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Added for PostAsJsonAsync and ReadFromJsonAsync

namespace JustiSafe.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ============================================================
        // 1. INICIO DE SESIÓN (LOGIN)
        // ============================================================

        // GET: Muestra la pantalla y limpia cualquier sesión anterior
        public async Task<IActionResult> Login()
        {
            // ESTA LÍNEA ES LA MAGIA:
            // Si alguien entra a esta pantalla, borramos cualquier sesión previa automáticamente.
            // Esto evita que salga el botón "Salir" o el usuario arriba si acabas de arrancar la app.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View();
        }

        // POST: Procesa las credenciales
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient("GatewayClient");
            var loginDto = new { Username = username, Password = password };
            
            var response = await client.PostAsJsonAsync("/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // Crear la "identificación" del usuario (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.Username), // Guardamos el ID (ej: JUD-8492)
                    new Claim(ClaimTypes.Role, result.Role),
                    new Claim("UserId", result.UserId.ToString()),
                    new Claim("JWT", result.Token) // Guardamos el Token para usarlo luego
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Iniciar sesión (crear la cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home"); // Al entrar, va al Dashboard
            }

            ViewBag.Error = "Credencial o contraseña incorrectos (Microservicio Identity)";
            return View();
        }

        // ============================================================
        // 2. REGISTRO (CREAR CUENTA)
        // ============================================================

        // GET: Muestra el formulario
        public IActionResult Register()
        {
            return View();
        }

        // POST: Recibe nombres y contraseña para crear el usuario
        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string password)
        {
            try
            {
                // TRUCO PARA ADMIN: 
                // Si el Nombre es "Super" y Apellido "Admin" -> Rol Admin.
                // Cualquier otro nombre -> Rol Juez.
                string role = (firstName.Equals("Super", StringComparison.OrdinalIgnoreCase) &&
                               lastName.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ? "Admin" : "Juez";

                var registerDto = new { FirstName = firstName, LastName = lastName, Password = password, Role = role };
                
                var client = _httpClientFactory.CreateClient("GatewayClient");
                var response = await client.PostAsJsonAsync("/auth/register", registerDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                    // Enviamos el usuario generado (ej: ADM-1234 o JUD-5678) a la vista
                    ViewBag.SuccessMessage = result.Username;
                    return View();
                }
                else
                {
                    ViewBag.Error = "Error en el registro";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // Clases DTO internas para deserializar respuestas
        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public string Username { get; set; }
            public int UserId { get; set; }
        }

        public class RegisterResponse
        {
            public string Message { get; set; }
            public string Username { get; set; }
        }

        // ============================================================
        // 3. CERRAR SESIÓN (LOGOUT)
        // ============================================================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}