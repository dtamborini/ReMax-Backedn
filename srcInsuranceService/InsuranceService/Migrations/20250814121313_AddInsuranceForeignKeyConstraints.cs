using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceService.Migrations
{
    /// <inheritdoc />
    public partial class AddInsuranceForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Insurances_PolicyDocumentId",
                table: "Insurances",
                column: "PolicyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_ReceiptDocumentId",
                table: "Insurances",
                column: "ReceiptDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLimits_InsuranceId",
                table: "InsuranceLimits",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDeductibles_InsuranceId",
                table: "InsuranceDeductibles",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsWorksPlans_InsuranceAccidentId",
                table: "InsuranceAccidentsWorksPlans",
                column: "InsuranceAccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsWorksPlans_WorkPlanId",
                table: "InsuranceAccidentsWorksPlans",
                column: "WorkPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsIssues_InsuranceAccidentId",
                table: "InsuranceAccidentsIssues",
                column: "InsuranceAccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsIssues_IssueId",
                table: "InsuranceAccidentsIssues",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidents_BuildingId",
                table: "InsuranceAccidents",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidents_InsuranceId",
                table: "InsuranceAccidents",
                column: "InsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceAccidents_Insurances_InsuranceId",
                table: "InsuranceAccidents",
                column: "InsuranceId",
                principalTable: "Insurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceAccidentsIssues_InsuranceAccidents_InsuranceAccidentId",
                table: "InsuranceAccidentsIssues",
                column: "InsuranceAccidentId",
                principalTable: "InsuranceAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceAccidentsWorksPlans_InsuranceAccidents_InsuranceAccidentId",
                table: "InsuranceAccidentsWorksPlans",
                column: "InsuranceAccidentId",
                principalTable: "InsuranceAccidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceDeductibles_Insurances_InsuranceId",
                table: "InsuranceDeductibles",
                column: "InsuranceId",
                principalTable: "Insurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceLimits_Insurances_InsuranceId",
                table: "InsuranceLimits",
                column: "InsuranceId",
                principalTable: "Insurances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceAccidents_Insurances_InsuranceId",
                table: "InsuranceAccidents");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceAccidentsIssues_InsuranceAccidents_InsuranceAccidentId",
                table: "InsuranceAccidentsIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceAccidentsWorksPlans_InsuranceAccidents_InsuranceAccidentId",
                table: "InsuranceAccidentsWorksPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceDeductibles_Insurances_InsuranceId",
                table: "InsuranceDeductibles");

            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceLimits_Insurances_InsuranceId",
                table: "InsuranceLimits");

            migrationBuilder.DropIndex(
                name: "IX_Insurances_PolicyDocumentId",
                table: "Insurances");

            migrationBuilder.DropIndex(
                name: "IX_Insurances_ReceiptDocumentId",
                table: "Insurances");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceLimits_InsuranceId",
                table: "InsuranceLimits");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceDeductibles_InsuranceId",
                table: "InsuranceDeductibles");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidentsWorksPlans_InsuranceAccidentId",
                table: "InsuranceAccidentsWorksPlans");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidentsWorksPlans_WorkPlanId",
                table: "InsuranceAccidentsWorksPlans");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidentsIssues_InsuranceAccidentId",
                table: "InsuranceAccidentsIssues");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidentsIssues_IssueId",
                table: "InsuranceAccidentsIssues");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidents_BuildingId",
                table: "InsuranceAccidents");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceAccidents_InsuranceId",
                table: "InsuranceAccidents");
        }
    }
}
