using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceEtudiante.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClasseId",
                table: "Utilisateurs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_ClasseId",
                table: "Utilisateurs",
                column: "ClasseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Classe_ClasseId",
                table: "Utilisateurs",
                column: "ClasseId",
                principalTable: "Classe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Classe_ClasseId",
                table: "Utilisateurs");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_ClasseId",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "ClasseId",
                table: "Utilisateurs");
        }
    }
}
