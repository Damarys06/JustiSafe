using JustiSafe.Data.Entities;
using System.Threading.Tasks;

namespace JustiSafe.Core.Interfaces
{
    public interface IUserService
    {
        // Ahora recibimos Nombre y Apellido para generar el usuario internamente
        Task<User> RegisterUser(string firstName, string lastName, string password, string role);

        Task<User?> Login(string username, string password);
    }
}