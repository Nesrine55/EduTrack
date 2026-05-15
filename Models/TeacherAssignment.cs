namespace PerformanceEtudiante.Models
{
    public class TeacherAssignment
    {
        public int Id { get; set; }
        public string EnseignantId { get; set; } = string.Empty;
        public ApplicationUser Enseignant { get; set; } = null!;
        public int ClasseId { get; set; }
        public Classe Classe { get; set; } = null!;
        public int MatiereId { get; set; }
        public Matiere Matiere { get; set; } = null!;
    }

}
