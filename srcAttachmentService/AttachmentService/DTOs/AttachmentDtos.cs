using System.ComponentModel.DataAnnotations;

namespace AttachmentService.DTOs;

/// <summary>
/// Response DTO for a single attachment
/// </summary>
public class AttachmentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AttachmentUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request DTO for getting multiple attachments by IDs
/// </summary>
public class GetAttachmentsByIdsRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one attachment ID is required")]
    [MaxLength(100, ErrorMessage = "Maximum 100 attachment IDs allowed per request")]
    public List<Guid> AttachmentIds { get; set; } = new List<Guid>();
}

/// <summary>
/// Response DTO for multiple attachments
/// </summary>
public class AttachmentsResponse
{
    public List<AttachmentResponse> Attachments { get; set; } = new List<AttachmentResponse>();
    public List<Guid> NotFoundIds { get; set; } = new List<Guid>();
}