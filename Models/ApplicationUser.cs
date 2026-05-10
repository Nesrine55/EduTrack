using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PerformanceEtudiante.Models
{
    public enum UserRole
    {
        Etudiant,
        Enseignant,
        Administrateur
    }

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telephone { get; set; }

        [StringLength(200)]
        public string? Adresse { get; set; }

        public DateTime DateNaissance { get; set; }

        public UserRole Role { get; set; } = UserRole.Etudiant;

        public string? PhotoProfil { get; set; }

        public DateTime DateInscription { get; set; } = DateTime.Now;

        public bool EstActif { get; set; } = true;

        public string NomComplet => $"{Prenom} {Nom}";
    }
}
