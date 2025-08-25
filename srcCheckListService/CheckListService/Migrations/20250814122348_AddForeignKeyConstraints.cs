using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckListService.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_CheckLists_CheckListGroups_CheckListGroupId",
                table: "CheckLists",
                column: "CheckListGroupId",
                principalTable: "CheckListGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckLists_CheckListGroups_CheckListGroupId",
                table: "CheckLists");
        }
    }
}
