using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustiSafe.Core.Interfaces;
using JustiSafe.Data;
using JustiSafe.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JustiSafe.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly JustiSafeDbContext _context;

        public CaseService(JustiSafeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Case>> GetAllCasesAsync(int userId, string role)
        {
            if (role == "Admin")
            {
                // El Admin (Consejo) ve TODOS los casos y quién es el juez asignado
                return await _context.Cases
                    .Include(c => c.Judge)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                // El Juez solo ve los casos que el sorteo le asignó
                return await _context.Cases
                    .Where(c => c.JudgeId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
        }

        public async Task<Case?> GetCaseByIdAsync(int id)
        {
            return await _context.Cases
                .Include(c => c.Verdicts)
                .Include(c => c.Judge)
                .FirstOrDefaultAsync(c => c.CaseId == id);
        }

        // LÓGICA DEL SORTEO DE JUECES
        public async Task CreateCaseAsync(Case newCase)
        {
            // 1. Buscar todos los usuarios que sean Jueces activos
            var judges = await _context.Users
                .Where(u => u.Role == "Juez" && u.IsActive)
                .ToListAsync();

            if (!judges.Any())
            {
                throw new Exception("No es posible crear el caso: No existen jueces registrados en el sistema para realizar el sorteo.");
            }

            // 2. Sorteo Aleatorio
            var random = new Random();
            var selectedJudge = judges[random.Next(judges.Count)];

            // 3. Asignar datos automáticos
            newCase.JudgeId = selectedJudge.UserId;
            newCase.CreatedAt = DateTime.Now;
            newCase.Status = "Sorteado";

            // 4. Generar Código Anónimo (Ej: CASE-2025-X9Y1)
            string year = DateTime.Now.Year.ToString();
            string randomPart = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            newCase.AnonCode = $"CASE-{year}-{randomPart}";

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCaseAsync(Case caseToUpdate)
        {
            _context.Cases.Update(caseToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCaseAsync(int id)
        {
            var caseToDelete = await _context.Cases.FindAsync(id);
            if (caseToDelete != null)
            {
                _context.Cases.Remove(caseToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}