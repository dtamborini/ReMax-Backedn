using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingService.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyAttachmentBuildingForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PolicyAttachments_BuildingId",
                table: "PolicyAttachments");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyAttachments_Buildings_BuildingId",
                table: "PolicyAttachments",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyAttachments_Buildings_BuildingId",
                table: "PolicyAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyAttachments_BuildingId",
                table: "PolicyAttachments",
                column: "BuildingId");
        }
    }
}
