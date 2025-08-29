using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Data.Context;
using AttachmentService.DTOs;

namespace AttachmentService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly AttachmentDbContext _context;
    private readonly ILogger<AttachmentsController> _logger;

    public AttachmentsController(AttachmentDbContext context, ILogger<AttachmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get a single attachment by ID
    /// </summary>
    /// <param name="id">Attachment ID</param>
    /// <returns>Attachment details</returns>
    /// <response code="200">Returns the attachment details</response>
    /// <response code="404">If the attachment is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AttachmentResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AttachmentResponse>> GetAttachment(Guid id)
    {
        try
        {
            var attachment = await _context.Attachments
                .Where(a => a.Id == id)
                .Select(a => new AttachmentResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    AttachmentUrl = a.AttachmentUrl,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                _logger.LogWarning("Attachment with ID {AttachmentId} not found", id);
                return NotFound($"Attachment with ID {id} not found");
            }

            _logger.LogInformation("Successfully retrieved attachment with ID {AttachmentId}", id);
            return Ok(attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attachment with ID {AttachmentId}", id);
            return StatusCode(500, "An error occurred while retrieving the attachment");
        }
    }

    /// <summary>
    /// Get multiple attachments by their IDs
    /// </summary>
    /// <param name="request">Request containing list of attachment IDs</param>
    /// <returns>List of attachments found and list of IDs not found</returns>
    /// <response code="200">Returns the attachments found and IDs not found</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(AttachmentsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AttachmentsResponse>> GetAttachmentsByIds([FromBody] GetAttachmentsByIdsRequest request)
    {
        try
        {
            if (request == null || request.AttachmentIds == null || !request.AttachmentIds.Any())
            {
                return BadRequest("At least one attachment ID is required");
            }

            // Remove duplicates from the request
            var uniqueIds = request.AttachmentIds.Distinct().ToList();

            if (uniqueIds.Count > 100)
            {
                return BadRequest("Maximum 100 attachment IDs allowed per request");
            }

            // Get attachments from database
            var attachments = await _context.Attachments
                .Where(a => uniqueIds.Contains(a.Id))
                .Select(a => new AttachmentResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    AttachmentUrl = a.AttachmentUrl,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            // Find which IDs were not found
            var foundIds = attachments.Select(a => a.Id).ToHashSet();
            var notFoundIds = uniqueIds.Where(id => !foundIds.Contains(id)).ToList();

            var response = new AttachmentsResponse
            {
                Attachments = attachments,
                NotFoundIds = notFoundIds
            };

            _logger.LogInformation(
                "Successfully retrieved {FoundCount} attachments out of {RequestedCount} requested", 
                attachments.Count, 
                uniqueIds.Count
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple attachments");
            return StatusCode(500, "An error occurred while retrieving the attachments");
        }
    }

    /// <summary>
    /// Get all attachments (paginated)
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Paginated list of attachments</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<object>> GetAllAttachments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var skip = (page - 1) * pageSize;

            var totalCount = await _context.Attachments.CountAsync();
            
            var attachments = await _context.Attachments
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .Select(a => new AttachmentResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    AttachmentUrl = a.AttachmentUrl,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Data = attachments
            };

            _logger.LogInformation(
                "Successfully retrieved page {Page} of attachments (size: {PageSize}, total: {TotalCount})", 
                page, 
                pageSize, 
                totalCount
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attachments list");
            return StatusCode(500, "An error occurred while retrieving attachments");
        }
    }
}