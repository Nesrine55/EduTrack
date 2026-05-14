using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceEtudiante.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeTeacherAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupeId",
                table: "Utilisateurs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Classe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groupe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClasseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groupe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groupe_Classe_ClasseId",
                        column: x => x.ClasseId,
                        principalTable: "Classe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnseignantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClasseId = table.Column<int>(type: "int", nullable: false),
                    GroupeId = table.Column<int>(type: "int", nullable: false),
                    MatiereId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAssignment_Classe_ClasseId",
                        column: x => x.ClasseId,
                        principalTable: "Classe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherAssignment_Groupe_GroupeId",
                        column: x => x.GroupeId,
                        principalTable: "Groupe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherAssignment_Matieres_MatiereId",
                        column: x => x.MatiereId,
                        principalTable: "Matieres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherAssignment_Utilisateurs_EnseignantId",
                        column: x => x.EnseignantId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_GroupeId",
                table: "Utilisateurs",
                column: "GroupeId");

            migrationBuilder.CreateIndex(
                name: "IX_Groupe_ClasseId",
                table: "Groupe",
                column: "ClasseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignment_ClasseId",
                table: "TeacherAssignment",
                column: "ClasseId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignment_EnseignantId",
                table: "TeacherAssignment",
                column: "EnseignantId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignment_GroupeId",
                table: "TeacherAssignment",
                column: "GroupeId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAssignment_MatiereId",
                table: "TeacherAssignment",
                column: "MatiereId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs",
                column: "GroupeId",
                principalTable: "Groupe",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "TeacherAssignment");

            migrationBuilder.DropTable(
                name: "Groupe");

            migrationBuilder.DropTable(
                name: "Classe");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_GroupeId",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "GroupeId",
                table: "Utilisateurs");
        }
    }
}
