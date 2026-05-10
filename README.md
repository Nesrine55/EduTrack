# 🎓 PerformanceÉtudiante — Application Web .NET MVC

**Application Web de Suivi de Performance et Réussite Étudiante**  
Équipe : Zarrouk Nesrine / Islem Adouni / Omar Turki

---

## 📋 Sprint 1 — Fonctionnalités réalisées

| US   | Thème           | Fonctionnalité                          | Statut |
|------|-----------------|-----------------------------------------|--------|
| US1  | Authentification | Connexion sécurisée + redirection       | ✅ Fait |
| US2  | Authentification | Gestion des rôles par l'admin           | ✅ Fait |
| US3  | Profil           | Modification du profil utilisateur      | ✅ Fait |

---

## 🏗️ Architecture MVC

```
PerformanceEtudiante/
├── Controllers/
│   ├── AccountController.cs     ← US1 (Login/Logout) + US2 (Rôles)
│   ├── ProfileController.cs     ← US3 (Modifier profil)
│   └── DashboardController.cs   ← Dashboard post-login
├── Models/
│   └── ApplicationUser.cs       ← Entité utilisateur (IdentityUser)
├── ViewModels/
│   └── Sprint1ViewModels.cs     ← LoginVM, GestionVM, ProfilVM
├── Data/
│   ├── ApplicationDbContext.cs  ← DbContext EF Core + Identity
│   ├── DbSeeder.cs              ← Seed rôles + admin/étudiant de test
│   └── Migration_Initiale.sql  ← Script SQL optionnel
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml         ← Page de connexion (US1)
│   │   ├── GestionUtilisateurs.cshtml ← Gestion rôles (US2)
│   │   └── AccesRefuse.cshtml
│   ├── Profile/
│   │   └── Index.cshtml         ← Formulaire profil (US3)
│   ├── Dashboard/
│   │   └── Index.cshtml         ← Dashboard après connexion
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Program.cs                   ← Configuration DI, Identity, routes
├── appsettings.json
└── PerformanceEtudiante.csproj
```

---

## 🚀 Installation et démarrage

### Prérequis
- **.NET SDK 8.0** → https://dotnet.microsoft.com/download/dotnet/8.0
- **SQL Server** (LocalDB, Express ou complet) ou **SQL Server Developer Edition**
- **Visual Studio 2022** (recommandé) ou **VS Code** + extension C#

---

### Étape 1 — Ouvrir le projet

```bash
# Ouvrir le dossier dans Visual Studio 2022
# Fichier → Ouvrir → Projet/Solution → PerformanceEtudiante.csproj
```

---

### Étape 2 — Configurer la chaîne de connexion

Dans `appsettings.json`, modifier si nécessaire :

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PerformanceEtudianteDB;Trusted_Connection=True;"
}
```

Pour SQL Server Express :
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PerformanceEtudianteDB;Trusted_Connection=True;"
```

---

### Étape 3 — Appliquer les migrations EF Core

Dans **Package Manager Console** (Visual Studio) :

```powershell
Add-Migration InitialCreate
Update-Database
```

Ou en ligne de commande :

```bash
cd PerformanceEtudiante
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> 💡 La base de données et les rôles (Admin/Enseignant/Etudiant) seront créés automatiquement au premier lancement grâce au `DbSeeder`.

---

### Étape 4 — Lancer l'application

```bash
dotnet run
# ou F5 dans Visual Studio
```

Accéder à : **https://localhost:5001** ou **http://localhost:5000**

---

## 🔐 Comptes de test (créés automatiquement)

| Rôle           | Email                       | Mot de passe     |
|----------------|-----------------------------|------------------|
| Administrateur | admin@performance.tn        | Admin@123456     |
| Étudiant       | etudiant@performance.tn     | Etudiant@123456  |

---

## 🔒 Sécurité implémentée (US1)

- ✅ Hachage des mots de passe (ASP.NET Identity - PBKDF2)
- ✅ Protection CSRF (AntiForgeryToken sur tous les formulaires POST)
- ✅ Blocage de compte après 5 tentatives échouées (15 min)
- ✅ Cookie HttpOnly + expiration 8h
- ✅ Redirection protégée (IsLocalUrl)
- ✅ Comptes désactivables par l'admin

---

## 🎭 Contrôle d'accès par rôle (US2)

| Route                              | Accès autorisé     |
|------------------------------------|--------------------|
| `/Account/Login`                   | Tous (anonyme)     |
| `/Dashboard/Index`                 | Tous (authentifié) |
| `/Account/GestionUtilisateurs`     | Administrateur     |
| `/Account/ModifierRole`            | Administrateur     |
| `/Account/ToggleActif`             | Administrateur     |
| `/Profile/Index`                   | Tous (authentifié) |

---

## 📦 Packages NuGet utilisés

```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore  8.0.0
Microsoft.EntityFrameworkCore.SqlServer            8.0.0
Microsoft.EntityFrameworkCore.Tools                8.0.0
Microsoft.EntityFrameworkCore.Design               8.0.0
Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation  8.0.0
```

---

## 📅 Prochains sprints

- **Sprint 2** : US4–US7 — Gestion des notes (saisie enseignant, consultation étudiant)
- **Sprint 3** : US8–US11 — Suivi de performance + Tableau de bord
- **Sprint 4** : US12–US16 — Analyse de risque + Recommandations
- **Sprint 5** : US17–US25 — Chatbot, Notifications, Ressources pédagogiques
