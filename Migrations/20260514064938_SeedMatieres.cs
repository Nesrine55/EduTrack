using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerformanceEtudiante.Migrations
{
    /// <inheritdoc />
    public partial class SeedMatieres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Matieres",
                columns: new[] { "Id", "Coefficient", "Nom" },
                values: new object[,]
                {
                    { 1, 0, "Mathématiques" },
                    { 2, 0, "Physique" },
                    { 3, 0, "Informatique" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Matieres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Matieres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Matieres",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
