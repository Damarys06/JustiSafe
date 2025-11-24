using System.Security.Cryptography;
using System.Text;
using JustiSafe.Core.Interfaces;
using JustiSafe.Data;
using JustiSafe.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace JustiSafe.Core.Services
{
    public class UserService : IUserService
    {
        private readonly JustiSafeDbContext _context;

        public UserService(JustiSafeDbContext context)
        {
            _context = context;
        }

        // --- MÉTODO PRIVADO DE ENCRIPTACIÓN (HASHING) ---
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Registro: Recibe contraseña plana -> Guarda encriptada
        public async Task<User> RegisterUser(string username, string password, string role = "Juez")
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new Exception("El usuario ya existe");

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password), // <--- ¡ENCRIPTADO AQUÍ!
                Role = role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Login: Recibe contraseña plana -> Encripta y compara
        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var loginHash = HashPassword(password);

            if (user.PasswordHash == loginHash)
            {
                return user;
            }

            return null;
        }
    }
}