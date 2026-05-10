using Microsoft.AspNetCore.Identity;
using PerformanceEtudiante.Models;

namespace PerformanceEtudiante.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Créer les rôles
            string[] roles = { "Administrateur", "Enseignant", "Etudiant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Créer un administrateur par défaut
            var adminEmail = "admin@performance.tn";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Prenom = "Super",
                    Nom = "Admin",
                    Role = UserRole.Administrateur,
                    EmailConfirmed = true,
                    EstActif = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrateur");
                }
            }

            // Créer un étudiant de test
            var etudiantEmail = "etudiant@performance.tn";
            var etudiantUser = await userManager.FindByEmailAsync(etudiantEmail);

            if (etudiantUser == null)
            {
                etudiantUser = new ApplicationUser
                {
                    UserName = etudiantEmail,
                    Email = etudiantEmail,
                    Prenom = "Ahmed",
                    Nom = "Ben Ali",
                    Role = UserRole.Etudiant,
                    EmailConfirmed = true,
                    EstActif = true,
                    DateNaissance = new DateTime(2002, 5, 15)
                };

                var result = await userManager.CreateAsync(etudiantUser, "Etudiant@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(etudiantUser, "Etudiant");
                }
            }
        }
    }
}
