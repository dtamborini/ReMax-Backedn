using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkOrderService.Migrations
{
    /// <inheritdoc />
    public partial class AddInterventionAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterventionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterventionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_InterventionAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionAttachments_AttachmentId",
                table: "InterventionAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionAttachments_InterventionId",
                table: "InterventionAttachments",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionAttachments_InterventionId_AttachmentId",
                table: "InterventionAttachments",
                columns: new[] { "InterventionId", "AttachmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterventionAttachments");
        }
    }
}
