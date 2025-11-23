using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustiSafe.Data.Entities
{
    public class Case
    {
        [Key]
        public int CaseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty; // Título real (privado)

        [Required]
        public string Description { get; set; } = string.Empty;

        // ESTE ES EL CAMPO IMPORTANTE: Código anónimo visible (ej: "CASO-2025-X92")
        [Required]
        public string AnonCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Abierto"; // Abierto, Cerrado, En Revisión

        // Relación con el Juez (User)
        public int JudgeId { get; set; }
        [ForeignKey("JudgeId")]
        public User? Judge { get; set; }

        // Relación: Un caso puede tener varias actualizaciones de sentencia
        public ICollection<Verdict> Verdicts { get; set; } = new List<Verdict>();
    }
}