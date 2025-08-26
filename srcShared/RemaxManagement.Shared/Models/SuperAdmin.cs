using System.ComponentModel.DataAnnotations;

namespace RemaxManagement.Shared.Models;

/// <summary>
/// Super Amministratore del sistema - vive nello schema public
/// Può gestire tenant e accedere a funzionalità di sistema
/// </summary>
public class SuperAdmin
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required] 
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? FullName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Super admin role - sempre "SuperAdmin"
    /// </summary>
    public string Role => "SuperAdmin";
}