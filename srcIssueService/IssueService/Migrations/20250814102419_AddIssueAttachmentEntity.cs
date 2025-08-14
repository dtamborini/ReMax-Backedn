using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueService.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssueAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_IssueAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_AttachmentId",
                table: "IssueAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_IssueId",
                table: "IssueAttachments",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_IssueId_AttachmentId",
                table: "IssueAttachments",
                columns: new[] { "IssueId", "AttachmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueAttachments");
        }
    }
}
