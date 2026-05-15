namespace PerformanceEtudiante.Models
{
    public class Groupe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public ICollection<Classe> Classes { get; set; } = new List<Classe>();
    }
}