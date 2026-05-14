namespace PerformanceEtudiante.Models
{
    public class Classe
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public ICollection<Groupe> Groupes { get; set; } = new List<Groupe>();
    }

}
