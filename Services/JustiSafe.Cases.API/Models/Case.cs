using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustiSafe.Cases.API.Models
{
    public class Case
    {
        [Key]
        public int CaseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string AnonCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Abierto";

        public int JudgeId { get; set; }
        // Navigation to User removed to decouple.

        public ICollection<Verdict> Verdicts { get; set; } = new List<Verdict>();
    }
}
