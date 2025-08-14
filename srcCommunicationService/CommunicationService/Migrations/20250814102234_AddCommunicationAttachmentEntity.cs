using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicationService.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunicationAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunicationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunicationId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_CommunicationAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationAttachments_AttachmentId",
                table: "CommunicationAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationAttachments_CommunicationId",
                table: "CommunicationAttachments",
                column: "CommunicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationAttachments_CommunicationId_AttachmentId",
                table: "CommunicationAttachments",
                columns: new[] { "CommunicationId", "AttachmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationAttachments");
        }
    }
}
