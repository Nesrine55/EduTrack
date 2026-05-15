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

            Groupes = _context.Groupes.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Nom
            }),

            Classes = _context.Classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nom
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

    public async Task<IActionResult> Edit(int id)
    {
        var assignment = await _context.TeacherAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assignment == null) return NotFound();

        var enseignants = await _userManager.GetUsersInRoleAsync("Enseignant");

        var vm = new TeacherAssignmentViewModel
        {
            Id = assignment.Id,
            EnseignantId = assignment.EnseignantId,
            ClasseId = assignment.ClasseId,
            MatiereId = assignment.MatiereId,
            Enseignants = enseignants.Select(e => new SelectListItem { Value = e.Id, Text = e.UserName }),
            Classes = _context.Classes.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nom }),
            Groupes = _context.Groupes.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nom }),
            Matieres = _context.Matieres.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nom })
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TeacherAssignmentViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var assignment = await _context.TeacherAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            assignment.EnseignantId = vm.EnseignantId;
            assignment.ClasseId = vm.ClasseId;
            assignment.MatiereId = vm.MatiereId;

            _context.Update(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Affectation modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        var enseignants = await _userManager.GetUsersInRoleAsync("Enseignant");
        vm.Enseignants = enseignants.Select(e => new SelectListItem { Value = e.Id, Text = e.UserName });
        vm.Classes = _context.Classes.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nom });
        vm.Groupes = _context.Groupes.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Nom });
        vm.Matieres = _context.Matieres.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Nom });

        return View(vm);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var assignment = await _context.TeacherAssignments
            .Include(a => a.Enseignant)
            .Include(a => a.Classe)
            .Include(a => a.Matiere)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assignment == null) return NotFound();

        return View(assignment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var assignment = await _context.TeacherAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        _context.TeacherAssignments.Remove(assignment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Affectation supprimée avec succès.";
        return RedirectToAction(nameof(Index));
    }
}
