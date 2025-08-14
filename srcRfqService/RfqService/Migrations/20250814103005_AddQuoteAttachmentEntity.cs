using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfqService.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuoteAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_QuoteAttachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteAttachments_AttachmentId",
                table: "QuoteAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteAttachments_QuoteId",
                table: "QuoteAttachments",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteAttachments_QuoteId_AttachmentId",
                table: "QuoteAttachments",
                columns: new[] { "QuoteId", "AttachmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuoteAttachments");
        }
    }
}
