using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Insurances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Agent = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AgentPhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AgreementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InsurancePremium = table.Column<double>(type: "double precision", nullable: false),
                    Fractionation = table.Column<double>(type: "double precision", nullable: false),
                    Previsions = table.Column<string>(type: "text", nullable: true),
                    PolicyDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceiptDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAccidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PracticalStatus = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EstimatedDamage = table.Column<double>(type: "double precision", nullable: false),
                    AmountPaid = table.Column<double>(type: "double precision", nullable: false),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    InsuranceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAccidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceAccidents_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceDeductibles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Guarantee = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    InsuranceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceDeductibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceDeductibles_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Guarantee = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    InsuranceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceLimits_Insurances_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAccidentsIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    InsuranceAccidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAccidentsIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceAccidentsIssues_InsuranceAccidents_InsuranceAccidentId",
                        column: x => x.InsuranceAccidentId,
                        principalTable: "InsuranceAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAccidentsWorksPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InsuranceAccidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAccidentsWorksPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceAccidentsWorksPlans_InsuranceAccidents_InsuranceAccidentId",
                        column: x => x.InsuranceAccidentId,
                        principalTable: "InsuranceAccidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidents_BuildingId",
                table: "InsuranceAccidents",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidents_InsuranceId",
                table: "InsuranceAccidents",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsIssues_InsuranceAccidentId",
                table: "InsuranceAccidentsIssues",
                column: "InsuranceAccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsIssues_IssueId",
                table: "InsuranceAccidentsIssues",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsWorksPlans_InsuranceAccidentId",
                table: "InsuranceAccidentsWorksPlans",
                column: "InsuranceAccidentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAccidentsWorksPlans_WorkPlanId",
                table: "InsuranceAccidentsWorksPlans",
                column: "WorkPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDeductibles_InsuranceId",
                table: "InsuranceDeductibles",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLimits_InsuranceId",
                table: "InsuranceLimits",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_PolicyDocumentId",
                table: "Insurances",
                column: "PolicyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurances_ReceiptDocumentId",
                table: "Insurances",
                column: "ReceiptDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceAccidentsIssues");

            migrationBuilder.DropTable(
                name: "InsuranceAccidentsWorksPlans");

            migrationBuilder.DropTable(
                name: "InsuranceDeductibles");

            migrationBuilder.DropTable(
                name: "InsuranceLimits");

            migrationBuilder.DropTable(
                name: "InsuranceAccidents");

            migrationBuilder.DropTable(
                name: "Insurances");
        }
    }
}
