using JustiSafe.Identity.API.Data;
using JustiSafe.Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JustiSafe.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(IdentityDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

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

        [HttpGet("judges")]
        public async Task<IActionResult> GetJudges()
        {
            var judges = await _context.Users
                .Where(u => u.Role == "Juez" && u.IsActive)
                .Select(u => new { u.UserId, u.Username })
                .ToListAsync();
            return Ok(judges);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null) return Unauthorized("Usuario no encontrado");

            var loginHash = HashPassword(login.Password);
            if (user.PasswordHash != loginHash) return Unauthorized("Contraseña incorrecta");

            var token = GenerateJwtToken(user);
            return Ok(new { token, user.Role, user.Username, user.UserId });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            // Generar usuario aleatorio (lógica portada de UserService)
            string prefix = (register.Role == "Admin") ? "ADM" : "JUD";
            string randomCode = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            string generatedUsername = $"{prefix}-{randomCode}";

            while (await _context.Users.AnyAsync(u => u.Username == generatedUsername))
            {
                randomCode = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
                generatedUsername = $"{prefix}-{randomCode}";
            }

            var user = new User
            {
                Username = generatedUsername,
                FirstName = register.FirstName,
                LastName = register.LastName,
                PasswordHash = HashPassword(register.Password),
                Role = register.Role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario registrado", Username = generatedUsername });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SecretKeySuperSegura1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Juez";
    }
}
