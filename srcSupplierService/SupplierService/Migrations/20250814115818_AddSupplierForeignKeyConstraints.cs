using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierService.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SupplierBuildings_BuildingId",
                table: "SupplierBuildings",
                column: "BuildingId");
                
            // Creare indice e FK constraint per SupplierId (interna al microservizio)
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_SupplierBuildings_SupplierId"" 
                ON ""SupplierBuildings"" (""SupplierId"");
            ");
            
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_SupplierBuildings_Suppliers_SupplierId') THEN
                        ALTER TABLE ""SupplierBuildings"" 
                        ADD CONSTRAINT ""FK_SupplierBuildings_Suppliers_SupplierId"" 
                        FOREIGN KEY (""SupplierId"") 
                        REFERENCES ""Suppliers""(""Id"") 
                        ON DELETE CASCADE;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplierBuildings_BuildingId",
                table: "SupplierBuildings");
                
            // Rimuovere FK constraint e indice
            migrationBuilder.Sql(@"
                ALTER TABLE ""SupplierBuildings"" 
                DROP CONSTRAINT IF EXISTS ""FK_SupplierBuildings_Suppliers_SupplierId"";
            ");
            
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""IX_SupplierBuildings_SupplierId"";
            ");
        }
    }
}
