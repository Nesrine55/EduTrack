using PerformanceEtudiante.Models;
using System.ComponentModel.DataAnnotations;

namespace PerformanceEtudiante.ViewModels
{
    public class NoteViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Range(0, 20)]
        public double Valeur { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MatiereId { get; set; }

        [Required]
        public string EtudiantId { get; set; }

        public IEnumerable<Matiere> Matieres { get; set; }
        public IEnumerable<ApplicationUser> Etudiants { get; set; }
    }
}
