using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JustiSafe.Core.Interfaces;
using JustiSafe.Data.Entities;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace JustiSafe.Web.Controllers
{
    [Authorize]
    public class CasesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CasesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("GatewayClient");
            var token = User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("/cases");
            
            if (response.IsSuccessStatusCode)
            {
                var cases = await response.Content.ReadFromJsonAsync<List<Case>>();
                return View(cases);
            }
            return View(new List<Case>());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Case newCase)
        {
            var client = CreateClientWithToken();
            var createDto = new { Title = newCase.Title, Description = newCase.Description };
            
            var response = await client.PostAsJsonAsync("/cases", createDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Caso sorteado vía Microservicio.";
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("", "Error al crear caso en el microservicio.");
            return View(newCase);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"/cases/{id}");
             if (response.IsSuccessStatusCode)
            {
                var caseItem = await response.Content.ReadFromJsonAsync<Case>();
                return View(caseItem);
            }
            return NotFound();
        }
        
        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"/cases/{id}");
             if (response.IsSuccessStatusCode)
            {
                var caseItem = await response.Content.ReadFromJsonAsync<Case>();
                // Solo Admin o el Juez asignado pueden editar
                // (Validación básica, idealmente el API lo protege también)
                return View(caseItem);
            }
            return NotFound();
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Case caseToUpdate)
        {
            var client = CreateClientWithToken();
            var updateDto = new { 
                CaseId = id,
                Title = caseToUpdate.Title, 
                Description = caseToUpdate.Description,
                Status = caseToUpdate.Status 
            };
            
            var response = await client.PutAsJsonAsync($"/cases/{id}", updateDto);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("", "Error al actualizar el caso.");
            return View(caseToUpdate);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"/cases/{id}");
             if (response.IsSuccessStatusCode)
            {
                var caseItem = await response.Content.ReadFromJsonAsync<Case>();
                return View(caseItem);
            }
            return NotFound();
        }

        // POST: Delete Confirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"/cases/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Caso eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el caso.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}