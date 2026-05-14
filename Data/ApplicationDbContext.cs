using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PerformanceEtudiante.Models;

namespace PerformanceEtudiante.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Matiere> Matieres { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Classe> Classes { get; set; }
        public DbSet<Groupe> Groupes { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relations Matiere -> Notes
            builder.Entity<Matiere>()
                .HasMany(m => m.Notes)
                .WithOne(n => n.Matiere)
                .HasForeignKey(n => n.MatiereId);

            // Relation Note -> Etudiant
            builder.Entity<Note>()
                .HasOne(n => n.Etudiant)
                .WithMany()
                .HasForeignKey(n => n.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Note -> Enseignant
            builder.Entity<Note>()
                .HasOne(n => n.Enseignant)
                .WithMany()
                .HasForeignKey(n => n.EnseignantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Classe>()
               .HasMany(c => c.Groupes)
               .WithOne(g => g.Classe)
               .HasForeignKey(g => g.ClasseId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Groupe>()
                     .HasMany(g => g.Etudiants)
                     .WithOne(u => u.Groupe)
                     .HasForeignKey(u => u.GroupeId)
                     .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<TeacherAssignment>()
                  .HasOne(a => a.Classe)
                  .WithMany()
                  .HasForeignKey(a => a.ClasseId)
                  .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TeacherAssignment>()
                .HasOne(a => a.Groupe)
                .WithMany()
                .HasForeignKey(a => a.GroupeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TeacherAssignment>()
                .HasOne(a => a.Matiere)
                .WithMany()
                .HasForeignKey(a => a.MatiereId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TeacherAssignment>()
                .HasOne(a => a.Enseignant)
                .WithMany(u => u.TeacherAssignments)
                .HasForeignKey(a => a.EnseignantId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Classe>().ToTable("Classe");
            builder.Entity<Groupe>().ToTable("Groupe");
            builder.Entity<TeacherAssignment>().ToTable("TeacherAssignment");
            builder.Entity<ApplicationUser>().ToTable("Utilisateurs");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UtilisateursRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UtilisateursClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UtilisateursLogins");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RolesClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UtilisateursTokens");

            builder.Entity<Classe>().HasData(
                new Classe { Id = 1, Nom = "Seconde A" },
                new Classe { Id = 2, Nom = "Première B" }
            );
            builder.Entity<Groupe>().HasData(
                new Groupe { Id = 1, Nom = "Groupe 1", ClasseId = 1 },
                new Groupe { Id = 2, Nom = "Groupe 2", ClasseId = 1 },
                new Groupe { Id = 3, Nom = "Groupe 1", ClasseId = 2 }
            );
            builder.Entity<Matiere>().HasData(
                new Matiere { Id = 1, Nom = "Mathématiques" },
                new Matiere { Id = 2, Nom = "Physique" },
                new Matiere { Id = 3, Nom = "Informatique" }
            );


        }
    }
}