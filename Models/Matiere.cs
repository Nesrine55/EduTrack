using System.ComponentModel.DataAnnotations;

namespace PerformanceEtudiante.Models
{
    public class Matiere
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Range(1, 10)]
        public int Coefficient { get; set; }

        public ICollection<Note> Notes { get; set; }
    }
}
