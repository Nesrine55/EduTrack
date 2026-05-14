using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformanceEtudiante.Data;
using PerformanceEtudiante.Models;
using PerformanceEtudiante.ViewModels;

[Authorize(Roles = "Etudiant")]
public class StudentNotesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentNotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: StudentNotes
    public async Task<IActionResult> Index(int? matiereId)
    {
        var userId = _userManager.GetUserId(User);
        var notesQuery = _context.Notes
            .Include(n => n.Matiere)
            .Where(n => n.EtudiantId == userId);

        if (matiereId.HasValue)
            notesQuery = notesQuery.Where(n => n.MatiereId == matiereId);

        var notes = await notesQuery.ToListAsync();

        var matieres = await _context.Matieres.ToListAsync();

        var grouped = notes.GroupBy(n => n.Matiere.Nom)
            .Select(g => new
            {
                Matiere = g.Key,
                Moyenne = g.Average(n => n.Valeur)
            }).ToList();

        var vm = new StudentNotesViewModel
        {
            Notes = notes,
            Moyenne = notes.Any() ? notes.Average(n => n.Valeur) : 0,
            MeilleureMatiere = grouped.OrderByDescending(g => g.Moyenne).FirstOrDefault()?.Matiere,
            PireMatiere = grouped.OrderBy(g => g.Moyenne).FirstOrDefault()?.Matiere,
            NombreMatieres = matieres.Count
        };

        ViewBag.Matieres = matieres;
        ViewBag.SelectedMatiere = matiereId;

        return View(vm);
    }

    // GET: StudentNotes/History
    public async Task<IActionResult> History()
    {
        var userId = _userManager.GetUserId(User);
        var notes = await _context.Notes
            .Include(n => n.Matiere)
            .Where(n => n.EtudiantId == userId && n.AncienneValeur != null)
            .OrderByDescending(n => n.DateModification)
            .ToListAsync();

        return View(notes);
    }
}
