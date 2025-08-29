using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingService.Data.Context;
using BuildingService.DTOs;
using System.Text.Json;
using System.Text;

namespace BuildingService.Controllers;


[ApiController] 
[Authorize]
[Route("api/[controller]")]
public class BuildingsController : ControllerBase
{
    private readonly BuildingDbContext _context;
    private readonly ILogger<BuildingsController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public BuildingsController(BuildingDbContext context, ILogger<BuildingsController> logger, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Get building details including IBANs and PolicyAttachments
    /// </summary>
    /// <param name="id">Building ID (GUID format)</param>
    /// <returns>Complete building details with associated IBANs and policy attachments</returns>
    /// <response code="200">Returns the building details with IBANs and attachments</response>
    /// <response code="404">If the building is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BuildingDetailResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BuildingDetailResponse>> GetBuildingDetail(Guid id)
    {
        try
        {
            // First, get the building
            var building = await _context.Buildings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (building == null)
            {
                _logger.LogWarning("Building with ID {BuildingId} not found", id);
                return NotFound($"Building with ID {id} not found");
            }

            // Then, get IBANs for this building
            var ibans = await _context.IBANs
                .Where(i => i.BuildingId == id)
                .Select(i => new IBANDto
                {
                    Id = i.Id,
                    Code = i.Code,
                    Description = i.Description
                })
                .ToListAsync();

            // Then, get PolicyAttachments for this building
            var attachments = await _context.PolicyAttachments
                .Where(a => a.BuildingId == id)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AttachmentId = a.AttachmentId,
                    AttachmentUrl = null // Will be populated below
                })
                .ToListAsync();

            // If we have attachments, get their URLs from AttachmentService
            if (attachments.Any())
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("AttachmentService");
                    
                    // Copy the tenant header from current request
                    if (Request.Headers.TryGetValue("Tenant", out var tenantHeader))
                    {
                        httpClient.DefaultRequestHeaders.Add("Tenant", tenantHeader.ToString());
                    }

                    // Copy the authorization header from current request
                    if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", authHeader.ToString());
                    }

                    // Prepare the request body with attachment IDs
                    var attachmentIds = attachments.Select(a => a.AttachmentId).ToList();
                    
                    var requestBody = new { attachmentIds = attachmentIds };
                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json"
                    );

                    // Call AttachmentService batch API
                    var response = await httpClient.PostAsync("/api/attachments/batch", jsonContent);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var attachmentServiceResponse = JsonSerializer.Deserialize<AttachmentServiceResponse>(
                            responseContent, 
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                        if (attachmentServiceResponse?.Attachments != null)
                        {
                            // Map the URLs back to our attachments
                            var urlMap = attachmentServiceResponse.Attachments.ToDictionary(a => a.Id, a => a.AttachmentUrl);
                            foreach (var attachment in attachments)
                            {
                                if (urlMap.TryGetValue(attachment.AttachmentId, out var url))
                                {
                                    attachment.AttachmentUrl = url;
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to get attachment URLs from AttachmentService: {StatusCode} - {ReasonPhrase}", 
                            response.StatusCode, response.ReasonPhrase);
                        attachments = new List<AttachmentDto>();
                        
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogWarning(httpEx, "AttachmentService is unavailable - continuing without attachment URLs");
                    // Continue without URLs - they will remain null
                    attachments = new List<AttachmentDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error calling AttachmentService for attachment URLs");
                    // Continue without URLs - they will remain null
                    attachments = new List<AttachmentDto>();
                    
                }
            }

            // Build the response
            var buildingResponse = new BuildingDetailResponse
            {
                Id = building.Id,
                Name = building.Name,
                Address = building.Address,
                Phone = building.Phone,
                Email = building.Email,
                Pec = building.Pec,
                FiscalCode = building.FiscalCode,
                VatCode = building.VatCode,
                Ibans = ibans,
                Attachments = attachments
            };

            _logger.LogInformation("Successfully retrieved building details for ID {BuildingId}", id);
            return Ok(buildingResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving building details for ID {BuildingId}", id);
            return StatusCode(500, "An error occurred while retrieving building details");
        }
    }

    /// <summary>
    /// Test endpoint - No authentication required
    /// </summary>
    [HttpGet("test")]
    [AllowAnonymous]
    public ActionResult<object> TestEndpoint()
    {
        return Ok(new { 
            message = "BuildingService is working",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }

    /// <summary>
    /// Get all buildings (basic info only)
    /// </summary>
    /// <returns>List of all buildings with basic information (without IBANs and attachments)</returns>
    /// <response code="200">Returns the list of buildings</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<object>>> GetBuildings()
    {
        try
        {
            var buildings = await _context.Buildings
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Address,
                    b.Phone,
                    b.Email,
                    b.FiscalCode,
                    b.VatCode
                })
                .ToListAsync();

            _logger.LogInformation("Successfully retrieved {Count} buildings", buildings.Count);
            return Ok(buildings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving buildings list");
            return StatusCode(500, "An error occurred while retrieving buildings list");
        }
    }
}