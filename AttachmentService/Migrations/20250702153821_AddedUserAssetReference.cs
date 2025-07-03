using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttachmentService.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAssetReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "Attachments");
        }
    }
}
