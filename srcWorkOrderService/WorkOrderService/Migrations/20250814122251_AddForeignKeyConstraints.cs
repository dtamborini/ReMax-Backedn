using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkOrderService.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_InterventionAttachments_Interventions_InterventionId",
                table: "InterventionAttachments",
                column: "InterventionId",
                principalTable: "Interventions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderAttachments_WorkOrders_WorkOrderId",
                table: "WorkOrderAttachments",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterventionAttachments_Interventions_InterventionId",
                table: "InterventionAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderAttachments_WorkOrders_WorkOrderId",
                table: "WorkOrderAttachments");
        }
    }
}
