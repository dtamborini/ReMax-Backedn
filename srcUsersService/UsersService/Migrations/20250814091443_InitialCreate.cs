using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DelegateId = table.Column<Guid>(type: "uuid", nullable: true),
                    DelegatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Residents_Residents_DelegateId",
                        column: x => x.DelegateId,
                        principalTable: "Residents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Residents_Residents_DelegatorId",
                        column: x => x.DelegatorId,
                        principalTable: "Residents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResidentPremises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Percentage = table.Column<double>(type: "double precision", precision: 18, scale: 4, nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    PremisesBuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentPremises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidentPremises_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPremises_BuildingId",
                table: "ResidentPremises",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPremises_PremisesBuildingId",
                table: "ResidentPremises",
                column: "PremisesBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPremises_Resident_Premises_Unique",
                table: "ResidentPremises",
                columns: new[] { "ResidentId", "PremisesBuildingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPremises_ResidentId",
                table: "ResidentPremises",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentPremises_ResidentId_BuildingId",
                table: "ResidentPremises",
                columns: new[] { "ResidentId", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Residents_DelegateId",
                table: "Residents",
                column: "DelegateId");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_DelegatorId",
                table: "Residents",
                column: "DelegatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_Email",
                table: "Residents",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Residents_Name",
                table: "Residents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_Name_Surname",
                table: "Residents",
                columns: new[] { "Name", "Surname" });

            migrationBuilder.CreateIndex(
                name: "IX_Residents_Surname",
                table: "Residents",
                column: "Surname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResidentPremises");

            migrationBuilder.DropTable(
                name: "Residents");
        }
    }
}
