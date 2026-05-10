-- ============================================================
-- Migration Initiale - PerformanceEtudianteDB
-- Sprint 1 : Authentification & Profil
-- Exécuter dans SQL Server Management Studio ou Azure Data Studio
-- ============================================================

-- Créer la base de données si elle n'existe pas
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PerformanceEtudianteDB')
BEGIN
    CREATE DATABASE PerformanceEtudianteDB;
END
GO

USE PerformanceEtudianteDB;
GO

-- Table des rôles
CREATE TABLE Roles (
    Id          NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name        NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);

-- Table des utilisateurs
CREATE TABLE Utilisateurs (
    Id                   NVARCHAR(450) NOT NULL PRIMARY KEY,
    Prenom               NVARCHAR(100) NOT NULL,
    Nom                  NVARCHAR(100) NOT NULL,
    Telephone            NVARCHAR(20) NULL,
    Adresse              NVARCHAR(200) NULL,
    DateNaissance        DATETIME2 NOT NULL DEFAULT '2000-01-01',
    Role                 INT NOT NULL DEFAULT 0,  -- 0=Etudiant, 1=Enseignant, 2=Admin
    PhotoProfil          NVARCHAR(MAX) NULL,
    DateInscription      DATETIME2 NOT NULL DEFAULT GETDATE(),
    EstActif             BIT NOT NULL DEFAULT 1,
    -- Identity fields
    UserName             NVARCHAR(256) NULL,
    NormalizedUserName   NVARCHAR(256) NULL,
    Email                NVARCHAR(256) NULL,
    NormalizedEmail      NVARCHAR(256) NULL,
    EmailConfirmed       BIT NOT NULL DEFAULT 0,
    PasswordHash         NVARCHAR(MAX) NULL,
    SecurityStamp        NVARCHAR(MAX) NULL,
    ConcurrencyStamp     NVARCHAR(MAX) NULL,
    PhoneNumber          NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled     BIT NOT NULL DEFAULT 0,
    LockoutEnd           DATETIMEOFFSET NULL,
    LockoutEnabled       BIT NOT NULL DEFAULT 1,
    AccessFailedCount    INT NOT NULL DEFAULT 0
);

-- Tables Identity auxiliaires
CREATE TABLE UtilisateursRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

CREATE TABLE UtilisateursClaims (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    UserId     NVARCHAR(450) NOT NULL,
    ClaimType  NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE
);

CREATE TABLE UtilisateursLogins (
    LoginProvider       NVARCHAR(128) NOT NULL,
    ProviderKey         NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId              NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE
);

CREATE TABLE RolesClaims (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    RoleId     NVARCHAR(450) NOT NULL,
    ClaimType  NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

CREATE TABLE UtilisateursTokens (
    UserId        NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name          NVARCHAR(128) NOT NULL,
    Value         NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE
);

-- Index
CREATE UNIQUE INDEX IX_Utilisateurs_NormalizedUserName ON Utilisateurs(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
CREATE UNIQUE INDEX IX_Utilisateurs_NormalizedEmail    ON Utilisateurs(NormalizedEmail)    WHERE NormalizedEmail    IS NOT NULL;
CREATE UNIQUE INDEX IX_Roles_NormalizedName            ON Roles(NormalizedName)            WHERE NormalizedName     IS NOT NULL;

PRINT 'Migration initiale terminée avec succès !'
GO
