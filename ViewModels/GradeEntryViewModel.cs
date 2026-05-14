using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class GradeEntryViewModel
{
    public int ClasseId { get; set; }
    public int GroupeId { get; set; }
    public int MatiereId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public decimal Valeur { get; set; }

    public IEnumerable<SelectListItem>? Classes { get; set; }
    public IEnumerable<SelectListItem>? Groupes { get; set; }
    public IEnumerable<SelectListItem>? Matieres { get; set; }
    public IEnumerable<SelectListItem>? Students { get; set; }
}
