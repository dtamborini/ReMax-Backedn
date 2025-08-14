using System;
using System.Collections.Generic;
using BuildingService.Data.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Pec = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FiscalCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    VatCode = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IBANs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IBANs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IBANs_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PremisesBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FatherId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeTags = table.Column<List<PremisesType>>(type: "jsonb", nullable: false, defaultValueSql: "'[]'::jsonb"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CustomData = table.Column<string>(type: "jsonb", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremisesBuildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremisesBuildings_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PremisesBuildings_PremisesBuildings_FatherId",
                        column: x => x.FatherId,
                        principalTable: "PremisesBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_FiscalCode",
                table: "Buildings",
                column: "FiscalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Name",
                table: "Buildings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_VatCode",
                table: "Buildings",
                column: "VatCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IBANs_BuildingId",
                table: "IBANs",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_IBANs_BuildingId_Code",
                table: "IBANs",
                columns: new[] { "BuildingId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IBANs_Code",
                table: "IBANs",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_PremisesBuildings_BuildingId",
                table: "PremisesBuildings",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_PremisesBuildings_BuildingId_Type",
                table: "PremisesBuildings",
                columns: new[] { "BuildingId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_PremisesBuildings_FatherId",
                table: "PremisesBuildings",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_PremisesBuildings_Type",
                table: "PremisesBuildings",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IBANs");

            migrationBuilder.DropTable(
                name: "PremisesBuildings");

            migrationBuilder.DropTable(
                name: "Buildings");
        }
    }
}
