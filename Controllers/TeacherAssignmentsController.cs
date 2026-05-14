using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PerformanceEtudiante.Data;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;

[Authorize(Roles = "Administrateur")]

public class TeacherAssignmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeacherAssignmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var assignments = _context.TeacherAssignments
            .Include(a => a.Enseignant)
            .Include(a => a.Classe)
            .Include(a => a.Groupe)
            .Include(a => a.Matiere);
        return View(await assignments.ToListAsync());
    }

    public async Task<IActionResult> Create(string? teacherId)
    {
        var enseignants = await _userManager.GetUsersInRoleAsync("Enseignant");

        if (!string.IsNullOrEmpty(teacherId))
        {
            enseignants = enseignants
                .Where(e => e.Id == teacherId)
                .ToList();
        }
        var vm = new TeacherAssignmentViewModel
        {
            Enseignants = enseignants.Select(e => new SelectListItem
            {
                Value = e.Id,
                Text = e.UserName
            }),

            Classes = _context.Classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nom
            }),

            Groupes = _context.Groupes.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Nom
            }),

            Matieres = _context.Matieres.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nom
            })
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherAssignmentViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var exists = await _context.TeacherAssignments.AnyAsync(a =>
                a.EnseignantId == vm.EnseignantId &&
                a.ClasseId == vm.ClasseId &&
                a.GroupeId == vm.GroupeId &&
                a.MatiereId == vm.MatiereId);

            if (exists)
            {
                TempData["Error"] = "Cette affectation existe déjà.";
                return RedirectToAction(nameof(Index));
            }

            var assignment = new TeacherAssignment
            {
                EnseignantId = vm.EnseignantId,
                ClasseId = vm.ClasseId,
                GroupeId = vm.GroupeId,
                MatiereId = vm.MatiereId
            };

            _context.TeacherAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Affectation ajoutée avec succès.";

            return RedirectToAction(nameof(Index));
        }
        // Recharger les listes déroulantes si erreur
        var enseignants = await _userManager.GetUsersInRoleAsync("Enseignant");
        vm.Enseignants = enseignants.Select(e => new SelectListItem { Value = e.Id, Text = e.UserName });
        vm.Classes = _context.Classes.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Nom
        });

        vm.Groupes = _context.Groupes.Select(g => new SelectListItem
        {
            Value = g.Id.ToString(),
            Text = g.Nom
        });

        vm.Matieres = _context.Matieres.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = m.Nom
        });
        return View(vm);
    }

    // Ajoutez Edit/Delete selon le même modèle
}
