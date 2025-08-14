using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckListService.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckListEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckListGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Complete = table.Column<bool>(type: "boolean", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RequiredDownload = table.Column<bool>(type: "boolean", nullable: false),
                    ReadReceipt = table.Column<bool>(type: "boolean", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuildingAttachmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CheckListGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckLists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckListGroups_Complete",
                table: "CheckListGroups",
                column: "Complete");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListGroups_Name",
                table: "CheckListGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListGroups_WorkOrderId",
                table: "CheckListGroups",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_AttachmentId",
                table: "CheckLists",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_BuildingAttachmentId",
                table: "CheckLists",
                column: "BuildingAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_CheckListGroupId",
                table: "CheckLists",
                column: "CheckListGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_Name",
                table: "CheckLists",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_Required",
                table: "CheckLists",
                column: "Required");

            migrationBuilder.CreateIndex(
                name: "IX_CheckLists_Type",
                table: "CheckLists",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckListGroups");

            migrationBuilder.DropTable(
                name: "CheckLists");
        }
    }
}
