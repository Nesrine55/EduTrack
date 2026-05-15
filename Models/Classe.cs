namespace PerformanceEtudiante.Models
{
    public class Classe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public int GroupeId { get; set; }
        public Groupe Groupe { get; set; } = null!;

        public ICollection<ApplicationUser> Etudiants { get; set; } = new List<ApplicationUser>();
    }
}