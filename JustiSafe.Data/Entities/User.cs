using System.ComponentModel.DataAnnotations;

namespace JustiSafe.Data.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Roles: "Admin", "Juez". Por defecto será Juez.
        public string Role { get; set; } = "Juez";

        public bool IsActive { get; set; } = true;

        // Relación: Un juez puede tener muchos casos asignados
        public ICollection<Case> AssignedCases { get; set; } = new List<Case>();
    }
}