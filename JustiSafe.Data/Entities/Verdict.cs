using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustiSafe.Data.Entities
{
    public class Verdict
    {
        [Key]
        public int VerdictId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty; // El texto de la sentencia

        public DateTime DateIssued { get; set; } = DateTime.Now;

        public bool IsFinal { get; set; } = false; // Si es true, ya es la sentencia final

        // Relación con el Caso
        public int CaseId { get; set; }
        [ForeignKey("CaseId")]
        public Case? Case { get; set; }
    }
}