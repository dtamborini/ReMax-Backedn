using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceService.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlans_PremisesBuildingId",
                table: "MaintenancePlans",
                column: "PremisesBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlans_SupplierId",
                table: "MaintenancePlans",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_MaintenancePlanId",
                table: "Deadlines",
                column: "MaintenancePlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deadlines_MaintenancePlans_MaintenancePlanId",
                table: "Deadlines",
                column: "MaintenancePlanId",
                principalTable: "MaintenancePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deadlines_MaintenancePlans_MaintenancePlanId",
                table: "Deadlines");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlans_PremisesBuildingId",
                table: "MaintenancePlans");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlans_SupplierId",
                table: "MaintenancePlans");

            migrationBuilder.DropIndex(
                name: "IX_Deadlines_MaintenancePlanId",
                table: "Deadlines");
        }
    }
}
