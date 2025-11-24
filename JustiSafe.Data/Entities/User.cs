using System.ComponentModel.DataAnnotations;

namespace JustiSafe.Data.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // Este será el código aleatorio (Ej: "JUD-X921") que el sistema genera
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // DATOS REALES (Solo visibles en Base de Datos por seguridad)
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = "Juez";

        public bool IsActive { get; set; } = true;

        // Relación con los casos
        public ICollection<Case> AssignedCases { get; set; } = new List<Case>();
    }
}