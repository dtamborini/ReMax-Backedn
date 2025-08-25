using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicationService.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunicationForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CommunicationAttachments_CommunicationId",
                table: "CommunicationAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_Communications_BuildingId",
                table: "Communications",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunicationAttachments_Communications_CommunicationId",
                table: "CommunicationAttachments",
                column: "CommunicationId",
                principalTable: "Communications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunicationAttachments_Communications_CommunicationId",
                table: "CommunicationAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Communications_BuildingId",
                table: "Communications");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationAttachments_CommunicationId",
                table: "CommunicationAttachments",
                column: "CommunicationId");
        }
    }
}
