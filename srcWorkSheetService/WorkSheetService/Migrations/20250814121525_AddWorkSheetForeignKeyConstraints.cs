using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkSheetService.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkSheetForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
                
            // Aggiungere FK constraint cross-microservice verso Buildings
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_WorkSheets_Buildings_BuildingId') THEN
                        ALTER TABLE ""WorkSheets"" 
                        ADD CONSTRAINT ""FK_WorkSheets_Buildings_BuildingId"" 
                        FOREIGN KEY (""BuildingId"") 
                        REFERENCES ""Buildings""(""Id"") 
                        ON DELETE RESTRICT;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
                
            // Rimuovere FK constraint
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkSheets"" 
                DROP CONSTRAINT IF EXISTS ""FK_WorkSheets_Buildings_BuildingId"";
            ");
        }
    }
}
