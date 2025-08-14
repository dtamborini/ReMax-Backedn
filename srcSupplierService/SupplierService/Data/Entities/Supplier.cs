using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using SupplierService.Data.Enums;

namespace SupplierService.Data.Entities;

public class Supplier : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    [MaxLength(20)]
    public string VatCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string AtecoCode { get; set; } = string.Empty;

    [Required]
    public SupplierState State { get; set; } = SupplierState.PendingApproval;
}