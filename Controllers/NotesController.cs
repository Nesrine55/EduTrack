using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceEtudiante.Data;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        var matieres = await _context.Matieres.ToListAsync();
        var etudiants = await _userManager.GetUsersInRoleAsync("Etudiant");
        var vm = new NoteViewModel
        {
            Matieres = matieres,
            Etudiants = etudiants
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });

            foreach (var e in errors)
            {
                Console.WriteLine($"{e.Field}: {string.Join(", ", e.Errors)}");
            }

            vm.Matieres = await _context.Matieres.ToListAsync();
            vm.Etudiants = await _userManager.GetUsersInRoleAsync("Etudiant");

            return View(vm);
        }

        var note = new Note
        {
            Valeur = vm.Valeur,
            MatiereId = vm.MatiereId,
            EtudiantId = vm.EtudiantId,
            EnseignantId = _userManager.GetUserId(User),
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
    public async Task<IActionResult> TeacherEntry()
    {
        var user = await _userManager.GetUserAsync(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == user.Id)
            .Include(a => a.Classe)
            .Include(a => a.Groupe)
            .Include(a => a.Matiere)
            .ToListAsync();

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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherEntry(GradeEntryViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);

        var assignments = await _context.TeacherAssignments
            .Where(a => a.EnseignantId == user.Id)
            .ToListAsync();

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

        // IMPORTANT FIX HERE
        vm.Students = await _context.Users
            .Where(u => u.GroupeId == vm.GroupeId)
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

            if (!isAssigned)
            {
                ModelState.AddModelError("", "Affectation non autorisée.");
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
}
