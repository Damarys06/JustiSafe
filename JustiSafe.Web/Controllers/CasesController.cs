using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JustiSafe.Core.Interfaces;
using JustiSafe.Data.Entities;
using System.Threading.Tasks;
using System;

namespace JustiSafe.Web.Controllers
{
    [Authorize]
    public class CasesController : Controller
    {
        private readonly ICaseService _caseService;

        public CasesController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        // GET: Index (Dashboard de Casos)
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            // Obtenemos el rol actual (Si no tiene, asumimos Juez)
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Juez";

            var cases = await _caseService.GetAllCasesAsync(userId, userRole);
            return View(cases);
        }

        // GET: Create (SOLO ADMIN)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create (SOLO ADMIN)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Case newCase)
        {
            // Removemos validaciones de campos automáticos
            ModelState.Remove("AnonCode");
            ModelState.Remove("Judge");

            if (ModelState.IsValid)
            {
                try
                {
                    await _caseService.CreateCaseAsync(newCase);
                    TempData["Success"] = "Caso creado y sorteado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Si falla el sorteo (ej: no hay jueces), mostramos error
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(newCase);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null) return NotFound();
            return View(caseItem);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Case caseToUpdate)
        {
            if (id != caseToUpdate.CaseId) return NotFound();

            ModelState.Remove("AnonCode");
            ModelState.Remove("Judge");

            if (ModelState.IsValid)
            {
                var originalCase = await _caseService.GetCaseByIdAsync(id);
                if (originalCase != null)
                {
                    originalCase.Title = caseToUpdate.Title;
                    originalCase.Description = caseToUpdate.Description;
                    originalCase.Status = caseToUpdate.Status;
                    await _caseService.UpdateCaseAsync(originalCase);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(caseToUpdate);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null) return NotFound();
            return View(caseItem);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var caseItem = await _caseService.GetCaseByIdAsync(id);
            if (caseItem == null) return NotFound();
            return View(caseItem);
        }

        // POST: Delete Confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _caseService.DeleteCaseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}