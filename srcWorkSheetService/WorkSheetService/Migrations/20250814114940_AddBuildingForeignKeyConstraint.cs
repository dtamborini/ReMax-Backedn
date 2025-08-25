using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkSheetService.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingForeignKeyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSheets_BuildingId",
                table: "WorkSheets",
                column: "BuildingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkSheets_BuildingId",
                table: "WorkSheets");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
