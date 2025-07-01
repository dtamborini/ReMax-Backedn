using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttachmentService.Migrations
{
    /// <inheritdoc />
    public partial class externalKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccidentGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NotificationGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuoteGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RfqGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkOrderGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkSheetGuid",
                table: "Attachments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccidentGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ActivityGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "NotificationGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "QuoteGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "RfqGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "TicketGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "WorkOrderGuid",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "WorkSheetGuid",
                table: "Attachments");
        }
    }
}
