using System.ComponentModel.DataAnnotations;

namespace TenantService.DTOs;

public class CreateTenantRequest
{
    [Required]
    [StringLength(100)]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public List<string>? Domains { get; set; }
}

public class UpdateTenantRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<string>? Domains { get; set; }
}