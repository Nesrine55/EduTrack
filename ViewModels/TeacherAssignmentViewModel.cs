using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace PerformanceEtudiante.ViewModels
{
    public class TeacherAssignmentViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string EnseignantId { get; set; } = string.Empty;

        [Required]
        public int GroupeId { get; set; }

        [Required]
        public int ClasseId { get; set; }

        [Required]
        public int MatiereId { get; set; }

        public IEnumerable<SelectListItem>? Enseignants { get; set; }
        public IEnumerable<SelectListItem>? Groupes { get; set; }
        public IEnumerable<SelectListItem>? Classes { get; set; }
        public IEnumerable<SelectListItem>? Matieres { get; set; }
    }
}