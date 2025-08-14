using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierService.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    State = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAttachments_AttachmentId",
                table: "SupplierAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAttachments_BuildingId",
                table: "SupplierAttachments",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAttachments_ExpireDate",
                table: "SupplierAttachments",
                column: "ExpireDate");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAttachments_Name",
                table: "SupplierAttachments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAttachments_State",
                table: "SupplierAttachments",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierAttachments");
        }
    }
}
