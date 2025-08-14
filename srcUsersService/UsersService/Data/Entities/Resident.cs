using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemaxManagement.Shared.Data.Entities;

namespace UsersService.Data.Entities;

public class Resident : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    // FK to self for delegate (the person who delegates to this resident)
    public Guid? DelegateId { get; set; }
    
    [ForeignKey(nameof(DelegateId))]
    public virtual Resident? Delegate { get; set; }
    
    // FK to self for delegator (the person this resident delegates to)
    public Guid? DelegatorId { get; set; }
    
    [ForeignKey(nameof(DelegatorId))]
    public virtual Resident? Delegator { get; set; }
    
    // Navigation properties for the inverse relationships
    [InverseProperty(nameof(Delegate))]
    public virtual ICollection<Resident> DelegatedFrom { get; set; } = new List<Resident>();
    
    [InverseProperty(nameof(Delegator))]
    public virtual ICollection<Resident> DelegatedTo { get; set; } = new List<Resident>();
    
    // Navigation property for premises relationships
    public virtual ICollection<ResidentPremises> ResidentPremises { get; set; } = new List<ResidentPremises>();
}