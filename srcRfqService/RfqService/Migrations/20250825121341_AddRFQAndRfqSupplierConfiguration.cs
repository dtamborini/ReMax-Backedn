using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfqService.Migrations
{
    /// <inheritdoc />
    public partial class AddRFQAndRfqSupplierConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RfqSuppliers_RfqId",
                table: "RfqSuppliers");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "RFQs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_RfqSuppliers_RfqId_SupplierId",
                table: "RfqSuppliers",
                columns: new[] { "RfqId", "SupplierId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RfqSuppliers_SupplierId",
                table: "RfqSuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_BuildingId",
                table: "RFQs",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_Priority",
                table: "RFQs",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_WorkOrderId",
                table: "RFQs",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_WorkSheetId",
                table: "RFQs",
                column: "WorkSheetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RfqSuppliers_RfqId_SupplierId",
                table: "RfqSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_RfqSuppliers_SupplierId",
                table: "RfqSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_RFQs_BuildingId",
                table: "RFQs");

            migrationBuilder.DropIndex(
                name: "IX_RFQs_Priority",
                table: "RFQs");

            migrationBuilder.DropIndex(
                name: "IX_RFQs_WorkOrderId",
                table: "RFQs");

            migrationBuilder.DropIndex(
                name: "IX_RFQs_WorkSheetId",
                table: "RFQs");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "RFQs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_RfqSuppliers_RfqId",
                table: "RfqSuppliers",
                column: "RfqId");
        }
    }
}
