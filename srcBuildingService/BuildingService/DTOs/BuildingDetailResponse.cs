using System.ComponentModel.DataAnnotations;

namespace BuildingService.DTOs;

public class BuildingDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Pec { get; set; }
    public string FiscalCode { get; set; } = string.Empty;
    public string VatCode { get; set; } = string.Empty;
    public List<IBANDto> Ibans { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}

public class IBANDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class AttachmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid AttachmentId { get; set; }
    public string? AttachmentUrl { get; set; }
}

// DTOs for AttachmentService response
internal class AttachmentServiceResponse
{
    public List<AttachmentServiceItem> Attachments { get; set; } = new();
    public List<Guid> NotFoundIds { get; set; } = new();
}

internal class AttachmentServiceItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AttachmentUrl { get; set; } = string.Empty;
}