using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;

namespace PerformanceEtudiante.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        // ==========================================
        // US3 - Afficher le profil
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var vm = new ModifierProfilViewModel
            {
                Prenom = user.Prenom,
                Nom = user.Nom,
                Telephone = user.Telephone,
                Adresse = user.Adresse,
                DateNaissance = user.DateNaissance == default ? DateTime.Now.AddYears(-20) : user.DateNaissance,
                PhotoProfilActuelle = user.PhotoProfil
            };

            return View(vm);
        }

        // ==========================================
        // US3 - Modifier le profil
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ModifierProfilViewModel model)
        {
            // Retirer la validation du mot de passe si les champs sont vides
            if (string.IsNullOrEmpty(model.MotDePasseActuel))
            {
                ModelState.Remove("MotDePasseActuel");
                ModelState.Remove("NouveauMotDePasse");
                ModelState.Remove("ConfirmerMotDePasse");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Mettre à jour les informations de base
            user.Prenom = model.Prenom;
            user.Nom = model.Nom;
            user.Telephone = model.Telephone;
            user.Adresse = model.Adresse;
            user.DateNaissance = model.DateNaissance;

            // Gestion de la photo de profil
            if (model.PhotoProfil != null && model.PhotoProfil.Length > 0)
            {
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(model.PhotoProfil.FileName).ToLower();

                if (!extensions.Contains(ext))
                {
                    ModelState.AddModelError("PhotoProfil", "Seules les images JPG, PNG et GIF sont acceptées.");
                    model.PhotoProfilActuelle = user.PhotoProfil;
                    return View(model);
                }

                // Supprimer l'ancienne photo
                if (!string.IsNullOrEmpty(user.PhotoProfil))
                {
                    var ancienChemin = Path.Combine(_environment.WebRootPath, "uploads", "profils", user.PhotoProfil);
                    if (System.IO.File.Exists(ancienChemin))
                        System.IO.File.Delete(ancienChemin);
                }

                // Sauvegarder la nouvelle photo
                var dossier = Path.Combine(_environment.WebRootPath, "uploads", "profils");
                Directory.CreateDirectory(dossier);
                var nomFichier = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                var chemin = Path.Combine(dossier, nomFichier);

                using (var stream = new FileStream(chemin, FileMode.Create))
                {
                    await model.PhotoProfil.CopyToAsync(stream);
                }

                user.PhotoProfil = nomFichier;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                model.PhotoProfilActuelle = user.PhotoProfil;
                return View(model);
            }

            // Changement de mot de passe (optionnel)
            if (!string.IsNullOrEmpty(model.MotDePasseActuel) && !string.IsNullOrEmpty(model.NouveauMotDePasse))
            {
                var changeResult = await _userManager.ChangePasswordAsync(user, model.MotDePasseActuel, model.NouveauMotDePasse);
                if (!changeResult.Succeeded)
                {
                    foreach (var error in changeResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    model.PhotoProfilActuelle = user.PhotoProfil;
                    return View(model);
                }
                TempData["Succes"] = "Profil et mot de passe mis à jour avec succès !";
            }
            else
            {
                TempData["Succes"] = "Profil mis à jour avec succès !";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
