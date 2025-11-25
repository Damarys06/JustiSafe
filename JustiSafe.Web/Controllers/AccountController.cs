using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using JustiSafe.Core.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace JustiSafe.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
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
            var user = await _userService.Login(username, password);

            if (user != null)
            {
                // Crear la "identificación" del usuario (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username), // Guardamos el ID (ej: JUD-8492)
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Iniciar sesión (crear la cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home"); // Al entrar, va al Dashboard
            }

            ViewBag.Error = "Credencial o contraseña incorrectos";
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

                var newUser = await _userService.RegisterUser(firstName, lastName, password, role);

                // Enviamos el usuario generado (ej: ADM-1234 o JUD-5678) a la vista
                ViewBag.SuccessMessage = newUser.Username;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
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