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

        // Método privado de Hashing
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

        // LÓGICA DE REGISTRO CON USUARIO ALEATORIO
        public async Task<User> RegisterUser(string firstName, string lastName, string password, string role = "Juez")
        {
            // 1. Definir prefijo según rol
            string prefix = (role == "Admin") ? "ADM" : "JUD";

            // 2. Generar código aleatorio
            string randomCode = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            string generatedUsername = $"{prefix}-{randomCode}";

            // 3. Verificar unicidad (muy raro que se repita, pero por seguridad)
            while (await _context.Users.AnyAsync(u => u.Username == generatedUsername))
            {
                randomCode = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
                generatedUsername = $"{prefix}-{randomCode}";
            }

            // 4. Crear entidad
            var user = new User
            {
                Username = generatedUsername, // El sistema lo inventa
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = HashPassword(password), // Contraseña manual encriptada
                Role = role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

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