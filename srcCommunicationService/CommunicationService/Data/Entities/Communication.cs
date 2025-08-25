using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace CommunicationService.Data.Entities;

public class Communication : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    public Guid BuildingId { get; set; }
    
    //TODO: aggiungi residenti, fornitori,
}