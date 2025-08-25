using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSupplierAttachmentReplaceBuildingIdWithSupplierId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BuildingId",
                table: "SupplierAttachments",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierAttachments_BuildingId",
                table: "SupplierAttachments",
                newName: "IX_SupplierAttachments_SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierAttachments_Suppliers_SupplierId",
                table: "SupplierAttachments",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierAttachments_Suppliers_SupplierId",
                table: "SupplierAttachments");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "SupplierAttachments",
                newName: "BuildingId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierAttachments_SupplierId",
                table: "SupplierAttachments",
                newName: "IX_SupplierAttachments_BuildingId");
        }
    }
}
