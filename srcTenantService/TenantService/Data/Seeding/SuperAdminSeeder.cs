using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.Models;
using TenantService.Data.Context;

namespace TenantService.Data.Seeding;

public static class SuperAdminSeeder
{
    /// <summary>
    /// Seed del primo SuperAdmin nel sistema
    /// </summary>
    public static async Task SeedSuperAdminAsync(TenantManagementDbContext context)
    {
        // Controlla se esiste già un super admin
        var existingSuperAdmin = await context.SuperAdmins
            .AnyAsync(sa => sa.Username == "superadmin");

        if (!existingSuperAdmin)
        {
            var superAdmin = new SuperAdmin
            {
                Id = Guid.NewGuid(),
                Username = "superadmin",
                Email = "superadmin@remax.com",
                PasswordHash = HashPassword("SuperAdmin123!"),
                FullName = "Super Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.SuperAdmins.Add(superAdmin);
            await context.SaveChangesAsync();
            
            Console.WriteLine("✅ Super Admin created successfully");
            Console.WriteLine("   Username: superadmin");
            Console.WriteLine("   Password: SuperAdmin123!");
        }
        else
        {
            Console.WriteLine("ℹ️ Super Admin already exists");
        }
    }

    /// <summary>
    /// Hash della password usando SHA256 (stesso metodo usato in SuperAdminAuthService)
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }
}