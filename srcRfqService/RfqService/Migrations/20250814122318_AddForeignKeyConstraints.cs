using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RfqService.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_NegotiationAttachments_Negotiations_NegotiationId",
                table: "NegotiationAttachments",
                column: "NegotiationId",
                principalTable: "Negotiations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteAttachments_Quotes_QuoteId",
                table: "QuoteAttachments",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NegotiationAttachments_Negotiations_NegotiationId",
                table: "NegotiationAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteAttachments_Quotes_QuoteId",
                table: "QuoteAttachments");
        }
    }
}
