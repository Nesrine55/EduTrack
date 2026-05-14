namespace PerformanceEtudiante.Models
{
    public class Groupe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public int ClasseId { get; set; }
        public Classe Classe { get; set; } = null!;
        public ICollection<ApplicationUser> Etudiants { get; set; } = new List<ApplicationUser>();
    }

}
