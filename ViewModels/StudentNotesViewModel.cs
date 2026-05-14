using PerformanceEtudiante.Models;

namespace PerformanceEtudiante.ViewModels
{
    public class StudentNotesViewModel
    {
        public IEnumerable<Note> Notes { get; set; }
        public double Moyenne { get; set; }
        public string MeilleureMatiere { get; set; }
        public string PireMatiere { get; set; }
        public int NombreMatieres { get; set; }
    }
}
