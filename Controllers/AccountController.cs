using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;

namespace PerformanceEtudiante.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // US1 - Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.EstActif) { ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect."); return View(model); }
            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.MotDePasse, model.SeRappelerDeMoi, lockoutOnFailure: true);
            if (result.Succeeded) return RedirectToLocal(returnUrl) ?? RedirectToAction("Index", "Dashboard");
            if (result.IsLockedOut) { ModelState.AddModelError(string.Empty, "Compte bloqué après plusieurs tentatives. Réessayez dans 15 min."); return View(model); }
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() { await _signInManager.SignOutAsync(); return RedirectToAction("Login"); }

        // US2 - Liste utilisateurs
        [HttpGet]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> GestionUtilisateurs(string? recherche, string? filtreRole)
        {
            var users = await _userManager.Users.ToListAsync();
            var vm = new ListeUtilisateursViewModel();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleActuel = roles.FirstOrDefault() ?? "Aucun";
                if (!string.IsNullOrEmpty(recherche) && !user.NomComplet.Contains(recherche, StringComparison.OrdinalIgnoreCase) && !(user.Email ?? "").Contains(recherche, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(filtreRole) && roleActuel != filtreRole) continue;
                vm.Utilisateurs.Add(new GestionUtilisateurViewModel { Id = user.Id, Prenom = user.Prenom, Nom = user.Nom, Email = user.Email ?? "", RoleActuel = roleActuel, NouveauRole = roleActuel, EstActif = user.EstActif });
            }
            ViewBag.Recherche = recherche;
            ViewBag.FiltreRole = filtreRole;
            return View(vm);
        }

        // US2 - Créer utilisateur
        [HttpGet]
        [Authorize(Roles = "Administrateur")]
        public IActionResult CreerUtilisateur() => View(new CreerUtilisateurViewModel());

        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerUtilisateur(CreerUtilisateurViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await _userManager.FindByEmailAsync(model.Email) != null) { ModelState.AddModelError("Email", "Cette adresse email est déjà utilisée."); return View(model); }
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Prenom = model.Prenom,
                Nom = model.Nom,
                Telephone = model.Telephone,
                DateNaissance = model.DateNaissance ?? DateTime.Now.AddYears(-20),
                Role = model.Role switch { "Administrateur" => UserRole.Administrateur, "Enseignant" => UserRole.Enseignant, _ => UserRole.Etudiant },
                EmailConfirmed = true,
                EstActif = true,
                DateInscription = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, model.MotDePasse);
            if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(model); }
            if (await _roleManager.RoleExistsAsync(model.Role)) await _userManager.AddToRoleAsync(user, model.Role);
            TempData["Succes"] = $"Utilisateur {user.NomComplet} créé avec le rôle '{model.Role}' avec succès !";
            return RedirectToAction(nameof(GestionUtilisateurs));
        }

        // US2 - Modifier utilisateur
        [HttpGet]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> ModifierUtilisateur(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) { TempData["Erreur"] = "Utilisateur introuvable."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            var roles = await _userManager.GetRolesAsync(user);
            var vm = new DetailUtilisateurViewModel { Id = user.Id, Prenom = user.Prenom, Nom = user.Nom, Email = user.Email ?? "", Telephone = user.Telephone, DateNaissance = user.DateNaissance == default ? null : user.DateNaissance, RoleActuel = roles.FirstOrDefault() ?? "Etudiant", EstActif = user.EstActif, DateInscription = user.DateInscription, PhotoProfil = user.PhotoProfil };
            ViewBag.RolesDisponibles = new List<string> { "Administrateur", "Enseignant", "Etudiant" };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierUtilisateur(DetailUtilisateurViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) { TempData["Erreur"] = "Utilisateur introuvable."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            user.Prenom = model.Prenom; user.Nom = model.Nom; user.Telephone = model.Telephone; user.EstActif = model.EstActif;
            if (model.DateNaissance.HasValue) user.DateNaissance = model.DateNaissance.Value;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) { foreach (var e in updateResult.Errors) ModelState.AddModelError(string.Empty, e.Description); ViewBag.RolesDisponibles = new List<string> { "Administrateur", "Enseignant", "Etudiant" }; return View(model); }
            var rolesActuels = await _userManager.GetRolesAsync(user);
            if ((rolesActuels.FirstOrDefault() ?? "") != model.RoleActuel && await _roleManager.RoleExistsAsync(model.RoleActuel))
            {
                await _userManager.RemoveFromRolesAsync(user, rolesActuels);
                await _userManager.AddToRoleAsync(user, model.RoleActuel);
                user.Role = model.RoleActuel switch { "Administrateur" => UserRole.Administrateur, "Enseignant" => UserRole.Enseignant, _ => UserRole.Etudiant };
                await _userManager.UpdateAsync(user);
            }
            TempData["Succes"] = $"Profil de {user.NomComplet} modifié avec succès.";
            return RedirectToAction(nameof(GestionUtilisateurs));
        }

        // US2 - Modifier rôle rapide
        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierRole(string userId, string nouveauRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(nouveauRole)) { TempData["Erreur"] = "Paramètres invalides."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            var rolesActuels = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesActuels);
            await _userManager.AddToRoleAsync(user, nouveauRole);
            user.Role = nouveauRole switch { "Administrateur" => UserRole.Administrateur, "Enseignant" => UserRole.Enseignant, _ => UserRole.Etudiant };
            await _userManager.UpdateAsync(user);
            TempData["Succes"] = $"Rôle de {user.NomComplet} mis à jour en '{nouveauRole}'.";
            return RedirectToAction(nameof(GestionUtilisateurs));
        }

        // US2 - Toggle actif
        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActif(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { TempData["Erreur"] = "Utilisateur introuvable."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            user.EstActif = !user.EstActif;
            await _userManager.UpdateAsync(user);
            TempData["Succes"] = $"Compte de {user.NomComplet} {(user.EstActif ? "activé" : "désactivé")}.";
            return RedirectToAction(nameof(GestionUtilisateurs));
        }

        // US2 - Supprimer
        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerUtilisateur(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { TempData["Erreur"] = "Utilisateur introuvable."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == userId) { TempData["Erreur"] = "Vous ne pouvez pas supprimer votre propre compte."; return RedirectToAction(nameof(GestionUtilisateurs)); }
            var nom = user.NomComplet;
            await _userManager.DeleteAsync(user);
            TempData["Succes"] = $"Utilisateur {nom} supprimé avec succès.";
            return RedirectToAction(nameof(GestionUtilisateurs));
        }

        [AllowAnonymous]
        public IActionResult AccesRefuse() => View();

        private IActionResult? RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return null;
        }
    }
}
