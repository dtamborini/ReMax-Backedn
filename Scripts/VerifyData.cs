using System;
using Npgsql;

namespace ReMaxApi.Scripts
{
    public class VerifyDataScript
    {
        private static readonly string ConnectionString = "Host=localhost;Database=remax_management;Username=remax_user;Password=remax_password123;Port=5433";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("🔍 Verifying seeded data...");
            
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                // Check Buildings
                using var buildingCmd = new NpgsqlCommand(@"SELECT COUNT(*) FROM ""Buildings""", connection);
                var buildingCount = await buildingCmd.ExecuteScalarAsync();
                Console.WriteLine($"📊 Buildings: {buildingCount}");

                // Check Suppliers  
                using var supplierCmd = new NpgsqlCommand(@"SELECT COUNT(*) FROM ""Suppliers""", connection);
                var supplierCount = await supplierCmd.ExecuteScalarAsync();
                Console.WriteLine($"📊 Suppliers: {supplierCount}");

                // Check Residents
                using var residentCmd = new NpgsqlCommand(@"SELECT COUNT(*) FROM ""Residents""", connection);
                var residentCount = await residentCmd.ExecuteScalarAsync();
                Console.WriteLine($"📊 Residents: {residentCount}");

                // Show sample data
                Console.WriteLine("\n📋 Sample Buildings:");
                using var buildingDataCmd = new NpgsqlCommand(@"SELECT ""Name"", ""Address"" FROM ""Buildings"" LIMIT 3", connection);
                using var buildingReader = await buildingDataCmd.ExecuteReaderAsync();
                while (await buildingReader.ReadAsync())
                {
                    Console.WriteLine($"  • {buildingReader.GetString(0)} - {buildingReader.GetString(1)}");
                }
                buildingReader.Close();

                Console.WriteLine("\n📋 Sample Suppliers:");
                using var supplierDataCmd = new NpgsqlCommand(@"SELECT ""CompanyName"", ""Email"" FROM ""Suppliers"" LIMIT 3", connection);
                using var supplierReader = await supplierDataCmd.ExecuteReaderAsync();
                while (await supplierReader.ReadAsync())
                {
                    Console.WriteLine($"  • {supplierReader.GetString(0)} - {supplierReader.GetString(1)}");
                }
                supplierReader.Close();

                Console.WriteLine("\n📋 Sample Residents:");
                using var residentDataCmd = new NpgsqlCommand(@"SELECT ""Name"", ""Surname"", ""Email"" FROM ""Residents"" LIMIT 3", connection);
                using var residentReader = await residentDataCmd.ExecuteReaderAsync();
                while (await residentReader.ReadAsync())
                {
                    Console.WriteLine($"  • {residentReader.GetString(0)} {residentReader.GetString(1)} - {residentReader.GetString(2)}");
                }

                Console.WriteLine("\n✅ Data verification completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during verification: {ex.Message}");
            }
        }
    }
}