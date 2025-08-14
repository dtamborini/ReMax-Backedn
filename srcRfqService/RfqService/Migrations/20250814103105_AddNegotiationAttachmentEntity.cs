using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfqService.Migrations
{
    /// <inheritdoc />
    public partial class AddNegotiationAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NegotiationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NegotiationId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_NegotiationAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NegotiationAttachments_AttachmentId",
                table: "NegotiationAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NegotiationAttachments_NegotiationId",
                table: "NegotiationAttachments",
                column: "NegotiationId");

            migrationBuilder.CreateIndex(
                name: "IX_NegotiationAttachments_NegotiationId_AttachmentId",
                table: "NegotiationAttachments",
                columns: new[] { "NegotiationId", "AttachmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NegotiationAttachments");
        }
    }
}
