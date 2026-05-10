using System.ComponentModel.DataAnnotations;
using PerformanceEtudiante.Models;

namespace PerformanceEtudiante.ViewModels
{
    // ===== US1 - Connexion =====
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi")]
        public bool SeRappelerDeMoi { get; set; }
    }

    // ===== US2 - Gestion des rôles =====
    public class GestionUtilisateurViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Rôle actuel")]
        public string RoleActuel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Nouveau rôle")]
        public string NouveauRole { get; set; } = string.Empty;

        public bool EstActif { get; set; }
    }

    public class ListeUtilisateursViewModel
    {
        public List<GestionUtilisateurViewModel> Utilisateurs { get; set; } = new();
        public List<string> RolesDisponibles { get; set; } = new() { "Administrateur", "Enseignant", "Etudiant" };
    }

    // ===== US3 - Modification de profil =====
    public class ModifierProfilViewModel
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100, ErrorMessage = "Maximum 100 caractères")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100, ErrorMessage = "Maximum 100 caractères")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        [StringLength(200)]
        [Display(Name = "Adresse")]
        public string? Adresse { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime DateNaissance { get; set; }

        [Display(Name = "Photo de profil")]
        public IFormFile? PhotoProfil { get; set; }

        public string? PhotoProfilActuelle { get; set; }

        // Pour changement de mot de passe
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string? MotDePasseActuel { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string? NouveauMotDePasse { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("NouveauMotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string? ConfirmerMotDePasse { get; set; }
    }

    // ===== Dashboard =====
    public class DashboardViewModel
    {
        public string NomComplet { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? PhotoProfil { get; set; }
    }
}
