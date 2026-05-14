using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerformanceEtudiante.Migrations
{
    /// <inheritdoc />
    public partial class FixTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs");

            migrationBuilder.InsertData(
                table: "Classe",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Seconde A" },
                    { 2, "Première B" }
                });

            migrationBuilder.InsertData(
                table: "Groupe",
                columns: new[] { "Id", "ClasseId", "Nom" },
                values: new object[,]
                {
                    { 1, 1, "Groupe 1" },
                    { 2, 1, "Groupe 2" },
                    { 3, 2, "Groupe 1" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs",
                column: "GroupeId",
                principalTable: "Groupe",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs");

            migrationBuilder.DeleteData(
                table: "Groupe",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groupe",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Groupe",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classe",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Classe",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Groupe_GroupeId",
                table: "Utilisateurs",
                column: "GroupeId",
                principalTable: "Groupe",
                principalColumn: "Id");
        }
    }
}
