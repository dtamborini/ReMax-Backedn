using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueService.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachments_Issues_IssueId",
                table: "IssueAttachments",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueWorkPlans_Issues_IssueId",
                table: "IssueWorkPlans",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachments_Issues_IssueId",
                table: "IssueAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueWorkPlans_Issues_IssueId",
                table: "IssueWorkPlans");
        }
    }
}
