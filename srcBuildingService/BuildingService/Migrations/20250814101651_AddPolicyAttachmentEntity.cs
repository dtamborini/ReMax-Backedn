using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingService.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PolicyAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
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
                    table.PrimaryKey("PK_PolicyAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_AttachmentId",
                table: "PolicyAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_BuildingId",
                table: "PolicyAttachments",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_BuildingId_AttachmentId",
                table: "PolicyAttachments",
                columns: new[] { "BuildingId", "AttachmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_Name",
                table: "PolicyAttachments",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyAttachments");
        }
    }
}
