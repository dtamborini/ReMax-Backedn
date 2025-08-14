using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingService.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingAttachmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastModify = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    FatherId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildingAttachments_BuildingAttachments_FatherId",
                        column: x => x.FatherId,
                        principalTable: "BuildingAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildingAttachments_AttachmentId",
                table: "BuildingAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingAttachments_BuildingId",
                table: "BuildingAttachments",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingAttachments_FatherId",
                table: "BuildingAttachments",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingAttachments_Type",
                table: "BuildingAttachments",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingAttachments");
        }
    }
}
