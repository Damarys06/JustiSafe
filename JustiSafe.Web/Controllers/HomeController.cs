using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JustiSafe.Web.Controllers
{
    [Authorize] // Solo deja entrar si ya te logueaste
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}