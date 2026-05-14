using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PerformanceEtudiante.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 20)]
        public double Valeur { get; set; }

        [Required]
        public DateTime DateAjout { get; set; } = DateTime.Now;

        [Required]
        public string EtudiantId { get; set; }
        [ForeignKey("EtudiantId")]
        public ApplicationUser Etudiant { get; set; }

        [Required]
        public int MatiereId { get; set; }
        public Matiere Matiere { get; set; }

        [Required]
        public string EnseignantId { get; set; }
        [ForeignKey("EnseignantId")]
        public ApplicationUser Enseignant { get; set; }

        // Historique
        public double? AncienneValeur { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
