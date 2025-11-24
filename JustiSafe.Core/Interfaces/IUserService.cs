using JustiSafe.Data.Entities;
using System.Threading.Tasks;

namespace JustiSafe.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUser(string username, string password, string role);
        Task<User?> Login(string username, string password);
    }
}