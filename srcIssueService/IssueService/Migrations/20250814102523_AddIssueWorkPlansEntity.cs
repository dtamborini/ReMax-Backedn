using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueService.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueWorkPlansEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssueWorkPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueWorkPlans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueWorkPlans_IssueId",
                table: "IssueWorkPlans",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueWorkPlans_IssueId_WorkSheetId",
                table: "IssueWorkPlans",
                columns: new[] { "IssueId", "WorkSheetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueWorkPlans_WorkSheetId",
                table: "IssueWorkPlans",
                column: "WorkSheetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueWorkPlans");
        }
    }
}
