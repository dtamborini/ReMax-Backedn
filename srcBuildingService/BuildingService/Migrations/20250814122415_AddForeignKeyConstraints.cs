using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingService.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_BuildingAttachments_Buildings_BuildingId",
                table: "BuildingAttachments",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildingAttachments_Buildings_BuildingId",
                table: "BuildingAttachments");
        }
    }
}
