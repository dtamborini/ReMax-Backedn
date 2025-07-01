using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MappingService.Migrations
{
    /// <inheritdoc />
    public partial class AddedVersionCodeAndActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Mappings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Mappings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Mappings");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Mappings");
        }
    }
}
