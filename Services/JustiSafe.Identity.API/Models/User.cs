using System.ComponentModel.DataAnnotations;

namespace JustiSafe.Identity.API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // DATOS REALES (Solo visibles en Base de Datos por seguridad)
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = "Juez";

        public bool IsActive { get; set; } = true;
    }
}
