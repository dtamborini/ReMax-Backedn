using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ReMaxApi.Scripts
{
    public class SeedDataScript
    {
        private static readonly string ConnectionString = "Host=localhost;Database=remax_management;Username=remax_user;Password=remax_password123;Port=5433";
        private static readonly Random Random = new Random();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Starting data seeding...");
            
            try
            {
                await SeedBuildingsAsync();
                await SeedSuppliersAsync();
                await SeedResidentsAsync();
                
                Console.WriteLine("✅ Data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during seeding: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static async Task SeedBuildingsAsync()
        {
            Console.WriteLine("📊 Seeding Buildings...");
            
            var buildings = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Torre del Sole",
                    Address = "Via Roma 123, Milano",
                    Phone = "+39 02 1234567",
                    Pec = "torredelsole@pec.it",
                    Email = "info@torredelsole.it",
                    FiscalCode = "TRRDLS80A01F205X",
                    VatCode = "12345678901",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Residenza Bellavista",
                    Address = "Corso Venezia 456, Roma",
                    Phone = "+39 06 9876543",
                    Pec = "bellavista@pec.it", 
                    Email = "amministrazione@bellavista.it",
                    FiscalCode = "RSDBLL75B02H501Y",
                    VatCode = "98765432109",
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Palazzo Moderno",
                    Address = "Via Garibaldi 789, Napoli",
                    Phone = "+39 081 5551234",
                    Pec = "moderno@pec.it",
                    Email = "info@palazzomoderno.it", 
                    FiscalCode = "PLZMDR90C03F839Z",
                    VatCode = "11223344556",
                    CreatedAt = DateTime.UtcNow
                }
            };

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var building in buildings)
            {
                var sql = @"
                    INSERT INTO ""Buildings"" (""Id"", ""Name"", ""Address"", ""Phone"", ""Pec"", ""Email"", 
                                            ""FiscalCode"", ""VatCode"", ""CreatedAt"")
                    VALUES (@Id, @Name, @Address, @Phone, @Pec, @Email, @FiscalCode, @VatCode, @CreatedAt)";

                using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Id", building.Id);
                cmd.Parameters.AddWithValue("@Name", building.Name);
                cmd.Parameters.AddWithValue("@Address", building.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", building.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pec", building.Pec ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", building.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FiscalCode", building.FiscalCode);
                cmd.Parameters.AddWithValue("@VatCode", building.VatCode);
                cmd.Parameters.AddWithValue("@CreatedAt", building.CreatedAt);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"  ✓ Created building: {building.Name}");
            }
        }

        private static async Task SeedSuppliersAsync()
        {
            Console.WriteLine("📊 Seeding Suppliers...");
            
            var suppliers = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Elettrica Milano SRL",
                    Name = "Marco",
                    Surname = "Rossi",
                    Address = "Via Brera 45, Milano, 20121",
                    Description = "Servizi elettrici professionali",
                    Email = "info@elettricamilano.it",
                    Phone = "+39 02 1234567",
                    VatCode = "12345678901",
                    AtecoCode = "43.21.01",
                    State = 0, // Approved
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Idraulici Roma & C.",
                    Name = "Giuseppe",
                    Surname = "Bianchi",
                    Address = "Via del Corso 234, Roma, 00186",
                    Description = "Impianti idraulici e manutenzione",
                    Email = "contact@idrauliciroma.com",
                    Phone = "+39 06 9876543",
                    VatCode = "98765432109",
                    AtecoCode = "43.22.01",
                    State = 0, // Approved
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    CompanyName = "Pulizie Sud Italia",
                    Name = "Anna",
                    Surname = "Verdi",
                    Address = "Corso Umberto 567, Napoli, 80138",
                    Description = "Servizi di pulizia e sanificazione",
                    Email = "amministrazione@puliziesud.it",
                    Phone = "+39 081 5551234",
                    VatCode = "11223344556",
                    AtecoCode = "81.21.00",
                    State = 2, // PendingApproval
                    CreatedAt = DateTime.UtcNow
                }
            };

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var supplier in suppliers)
            {
                var sql = @"
                    INSERT INTO ""Suppliers"" (""Id"", ""CompanyName"", ""Name"", ""Surname"", ""Address"", ""Description"", 
                                            ""Email"", ""Phone"", ""VatCode"", ""AtecoCode"", ""State"", ""CreatedAt"")
                    VALUES (@Id, @CompanyName, @Name, @Surname, @Address, @Description, @Email, @Phone, @VatCode, @AtecoCode, @State, @CreatedAt)";

                using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Id", supplier.Id);
                cmd.Parameters.AddWithValue("@CompanyName", supplier.CompanyName);
                cmd.Parameters.AddWithValue("@Name", supplier.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Surname", supplier.Surname ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", supplier.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", supplier.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", supplier.Email);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@VatCode", supplier.VatCode);
                cmd.Parameters.AddWithValue("@AtecoCode", supplier.AtecoCode);
                cmd.Parameters.AddWithValue("@State", supplier.State);
                cmd.Parameters.AddWithValue("@CreatedAt", supplier.CreatedAt);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"  ✓ Created supplier: {supplier.CompanyName}");
            }
        }

        private static async Task SeedResidentsAsync()
        {
            Console.WriteLine("📊 Seeding Residents...");
            
            var residents = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Alessandro",
                    Surname = "Ferrari",
                    Phone = "+39 333 1234567",
                    Email = "alessandro.ferrari@email.com",
                    DelegateId = (Guid?)null,
                    DelegatorId = (Guid?)null,
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Giulia",
                    Surname = "Conti",
                    Phone = "+39 347 9876543",
                    Email = "giulia.conti@email.com",
                    DelegateId = (Guid?)null,
                    DelegatorId = (Guid?)null,
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Name = "Roberto",
                    Surname = "Esposito",
                    Phone = "+39 329 5551234",
                    Email = "roberto.esposito@email.com",
                    DelegateId = (Guid?)null,
                    DelegatorId = (Guid?)null,
                    CreatedAt = DateTime.UtcNow
                }
            };

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var resident in residents)
            {
                var sql = @"
                    INSERT INTO ""Residents"" (""Id"", ""Name"", ""Surname"", ""Phone"", ""Email"", 
                                            ""DelegateId"", ""DelegatorId"", ""CreatedAt"")
                    VALUES (@Id, @Name, @Surname, @Phone, @Email, @DelegateId, @DelegatorId, @CreatedAt)";

                using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Id", resident.Id);
                cmd.Parameters.AddWithValue("@Name", resident.Name);
                cmd.Parameters.AddWithValue("@Surname", resident.Surname);
                cmd.Parameters.AddWithValue("@Phone", resident.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", resident.Email);
                cmd.Parameters.AddWithValue("@DelegateId", resident.DelegateId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DelegatorId", resident.DelegatorId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAt", resident.CreatedAt);

                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"  ✓ Created resident: {resident.Name} {resident.Surname}");
            }
        }
    }
}