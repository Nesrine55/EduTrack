using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PerformanceEtudiante.ViewModels
{
    public class CreerUtilisateurViewModel
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        [Display(Name = "Adresse email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime? DateNaissance { get; set; }

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = "Etudiant";

        [Display(Name = "Classe")]
        public int? ClasseId { get; set; }

        [Display(Name = "Groupe")]
        public int? GroupeId { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Minimum 8 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation est requise")]
        [DataType(DataType.Password)]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmerMotDePasse { get; set; } = string.Empty;

        public List<string> RolesDisponibles { get; set; } = new() { "Administrateur", "Enseignant", "Etudiant" };

        public IEnumerable<SelectListItem>? Classes { get; set; }
        public IEnumerable<SelectListItem>? Groupes { get; set; }
    }

    public class DetailUtilisateurViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public DateTime? DateNaissance { get; set; }
        public string RoleActuel { get; set; } = string.Empty;
        public bool EstActif { get; set; }
        public DateTime DateInscription { get; set; }
        public string? PhotoProfil { get; set; }
    }
}
