using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceEtudiante.Data;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
[Authorize(Roles = "Enseignant")]
public class NotesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Notes
    public async Task<IActionResult> Index()
    {
        var notes = await _context.Notes
            .Include(n => n.Matiere)
            .Include(n => n.Etudiant)
            .ToListAsync();
        return View(notes);
    }

    // GET: Notes/Create
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == userId)
            .Include(a => a.Groupe)
            .ToListAsync();

        var matiereIds = assignments
            .Select(a => a.MatiereId)
            .Distinct()
            .ToList();

        var matieres = await _context.Matieres
            .Where(m => matiereIds.Contains(m.Id))
            .ToListAsync();

        var vm = new NoteViewModel
        {
            Matieres = matieres,
            Etudiants = new List<ApplicationUser>(),
            Groupes = assignments
                .Select(a => a.Groupe)
                .Distinct()
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Nom
                })
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteViewModel vm)
    {
        var userId = _userManager.GetUserId(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == userId)
            .Include(a => a.Groupe)
            .ToListAsync();

        var isAssigned = assignments.Any(a =>
            a.GroupeId == vm.GroupeId &&
            a.MatiereId == vm.MatiereId);

        var student = await _context.Users
            .Include(u => u.Groupe)
            .FirstOrDefaultAsync(u => u.Id == vm.EtudiantId);

        var isStudentInGroup = student?.GroupeId == vm.GroupeId;

        if (!isAssigned || !isStudentInGroup)
        {
            ModelState.AddModelError("", "Unauthorized assignment");
        }

        if (!ModelState.IsValid)
        {
            var matiereIds = assignments.Select(a => a.MatiereId).Distinct().ToList();

            vm.Matieres = await _context.Matieres
                .Where(m => matiereIds.Contains(m.Id))
                .ToListAsync();

            vm.Etudiants = new List<ApplicationUser>();

            vm.Groupes = assignments
                .Select(a => a.Groupe)
                .Distinct()
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Nom
                });

            return View(vm);
        }

        var note = new Note
        {
            Valeur = vm.Valeur,
            MatiereId = vm.MatiereId,
            EtudiantId = vm.EtudiantId,
            EnseignantId = userId,
            DateAjout = DateTime.Now
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Note ajoutée avec succès.";
        return RedirectToAction(nameof(Index));
    }


    // GET: Notes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();

        var vm = new NoteViewModel
        {
            Id = note.Id,
            Valeur = note.Valeur,
            MatiereId = note.MatiereId,
            EtudiantId = note.EtudiantId,
            Matieres = await _context.Matieres.ToListAsync(),
            Etudiants = await _userManager.GetUsersInRoleAsync("Etudiant")
        };
        return View(vm);
    }

    // POST: Notes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoteViewModel vm)
    {
        if (id != vm.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null) return NotFound();

            note.AncienneValeur = note.Valeur;
            note.Valeur = vm.Valeur;
            note.MatiereId = vm.MatiereId;
            note.EtudiantId = vm.EtudiantId;
            note.DateModification = DateTime.Now;

            _context.Update(note);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Note modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        vm.Matieres = await _context.Matieres.ToListAsync();
        vm.Etudiants = await _userManager.GetUsersInRoleAsync("Etudiant");
        return View(vm);
    }

    // GET: Notes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var note = await _context.Notes
            .Include(n => n.Matiere)
            .Include(n => n.Etudiant)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (note == null) return NotFound();
        return View(note);
    }

    // POST: Notes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note != null)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Note supprimée.";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Notes/TeacherEntry
    // GET: Notes/TeacherEntry
    public async Task<IActionResult> TeacherEntry()
    {
        var user = await _userManager.GetUserAsync(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == user.Id)
            .Include(a => a.Classe)
            .Include(a => a.Groupe)
            .Include(a => a.Matiere)
            .ToListAsync();

        ViewBag.TeacherAssignments = JsonSerializer.Serialize(assignments.Select(a => new
        {
            a.ClasseId,
            ClasseNom = a.Classe.Nom,
            a.GroupeId,
            GroupeNom = a.Groupe.Nom,
            a.MatiereId,
            MatiereNom = a.Matiere.Nom
        }));

        var vm = new GradeEntryViewModel
        {
            Classes = assignments
                .Select(a => a.Classe)
                .Distinct()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nom
                })
        };

        return View(vm);
    }

    // POST: Notes/TeacherEntry
    // POST: Notes/TeacherEntry
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherEntry(GradeEntryViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == user.Id)
            .Include(a => a.Classe)
            .Include(a => a.Groupe)
            .Include(a => a.Matiere)
            .ToListAsync();

        ViewBag.TeacherAssignments = JsonSerializer.Serialize(assignments.Select(a => new
        {
            a.ClasseId,
            ClasseNom = a.Classe.Nom,
            a.GroupeId,
            GroupeNom = a.Groupe.Nom,
            a.MatiereId,
            MatiereNom = a.Matiere.Nom
        }));

        vm.Classes = assignments
            .Select(a => a.Classe)
            .Distinct()
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nom
            });

        vm.Groupes = assignments
            .Where(a => a.ClasseId == vm.ClasseId)
            .Select(a => a.Groupe)
            .Distinct()
            .Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Nom
            });

        vm.Matieres = assignments
            .Where(a =>
                a.ClasseId == vm.ClasseId &&
                a.GroupeId == vm.GroupeId)
            .Select(a => a.Matiere)
            .Distinct()
            .Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Nom
            });

        vm.Students = await _context.Users
            .Include(u => u.Groupe)
            .Where(u => u.GroupeId == vm.GroupeId && u.Groupe.ClasseId == vm.ClasseId)
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            })
            .ToListAsync();

        if (ModelState.IsValid)
        {
            var isAssigned = assignments.Any(a =>
                a.ClasseId == vm.ClasseId &&
                a.GroupeId == vm.GroupeId &&
                a.MatiereId == vm.MatiereId);

            var student = await _context.Users
                .Include(u => u.Groupe)
                .FirstOrDefaultAsync(u => u.Id == vm.StudentId);

            var isStudentInGroup = student?.GroupeId == vm.GroupeId;
            var isStudentInClass = student?.Groupe?.ClasseId == vm.ClasseId;

            if (!isAssigned || !isStudentInGroup || !isStudentInClass)
            {
                ModelState.AddModelError("", "Unauthorized assignment");
                return View(vm);
            }

            var note = new Note
            {
                EtudiantId = vm.StudentId,
                MatiereId = vm.MatiereId,
                Valeur = (double)vm.Valeur,
                EnseignantId = user.Id,
                DateAjout = DateTime.Now
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Note enregistrée avec succès.";
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> StudentsByGroupe(int groupeId)
    {
        var userId = _userManager.GetUserId(User);

        var allowed = await _context.TeacherAssignments
            .AnyAsync(a => a.EnseignantId == userId && a.GroupeId == groupeId);

        if (!allowed) return Forbid();

        var students = await _context.Users
            .Where(u => u.GroupeId == groupeId && u.Role == UserRole.Etudiant)
            .Select(u => new { id = u.Id, name = u.UserName })
            .ToListAsync();

        return Json(students);
    }


}
