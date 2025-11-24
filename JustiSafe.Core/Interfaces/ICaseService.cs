using JustiSafe.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JustiSafe.Core.Interfaces
{
    public interface ICaseService
    {
        // Ahora recibe el rol para saber si devuelve todos (Admin) o solo los propios (Juez)
        Task<List<Case>> GetAllCasesAsync(int userId, string role);

        Task<Case?> GetCaseByIdAsync(int id);

        // Ya no pide el ID del juez, porque el sistema lo sortea internamente
        Task CreateCaseAsync(Case newCase);

        Task UpdateCaseAsync(Case caseToUpdate);
        Task DeleteCaseAsync(int id);
    }
}