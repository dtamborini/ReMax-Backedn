using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace AttachmentService.Data.Entities;

public class Attachment : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string AttachmentUrl { get; set; } = string.Empty;
}