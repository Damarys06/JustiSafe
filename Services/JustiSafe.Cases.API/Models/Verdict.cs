using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustiSafe.Cases.API.Models
{
    public class Verdict
    {
        [Key]
        public int VerdictId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime DateIssued { get; set; } = DateTime.Now;

        public bool IsFinal { get; set; } = false;

        public int CaseId { get; set; }
        [ForeignKey("CaseId")]
        public Case? Case { get; set; }
    }
}
