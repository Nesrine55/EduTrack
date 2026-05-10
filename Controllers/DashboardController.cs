using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;

namespace PerformanceEtudiante.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var roles = await _userManager.GetRolesAsync(user);
            var vm = new DashboardViewModel
            {
                NomComplet = user.NomComplet,
                Role = roles.FirstOrDefault() ?? "Etudiant",
                PhotoProfil = user.PhotoProfil
            };

            return View(vm);
        }
    }
}
