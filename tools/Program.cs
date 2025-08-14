using Npgsql;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = "Host=localhost;Database=condominium_management;Username=condominium_user;Port=5432;SSL Mode=Disable";
        
        Console.WriteLine("Testing PostgreSQL connection...");
        Console.WriteLine($"Connection string: {connectionString}");
        
        try 
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            using var command = new NpgsqlCommand("SELECT version()", connection);
            var result = await command.ExecuteScalarAsync();
            
            Console.WriteLine("SUCCESS: Connected to PostgreSQL!");
            Console.WriteLine($"Version: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: Failed to connect to PostgreSQL");
            Console.WriteLine($"Error: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }
}