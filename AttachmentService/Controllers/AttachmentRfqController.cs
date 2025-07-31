using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Models;
using AttachmentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;
using RemaxApi.Shared.Authentication.Services;

namespace AttachmentRfqController.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/rfqs/{uuidRfq}/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentRfqController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly IExternalAuthUserService _externalAuthUserService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;

        public AttachmentRfqController(
            UserClaimService userClaimService,
            IExternalAuthUserService externalAuthUserService,
            AttachmentDbContext context,
            IAttachmentFactoryService attachmentFactoryService,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _userClaimService = userClaimService;
            _externalAuthUserService = externalAuthUserService;
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _attachmentFactoryService = attachmentFactoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAttachments(
            [FromRoute] Guid uuidBuilding,
            [FromRoute] Guid uuidWorksheet,
            [FromRoute] Guid uuidRfq
        )
        {
            var attachments = await _context.Attachments
                .Where(
                    a => a.BuildingGuid == uuidBuilding &&
                    a.WorkSheetGuid == uuidWorksheet &&
                    a.RfqGuid == uuidRfq
                )
                .ToListAsync();

            if (!attachments.Any())
            {
                return NotFound($"Nessun attachment trovato per RFQ con GUID '{uuidWorksheet}'.");
            }

            attachments.ForEach(b => b.DeserializeComplexData());
            
            return Ok(new 
            {
                data = attachments,
                userInfo = new 
                {
                    userId = _externalAuthUserService.GetUserId(),
                    userName = _externalAuthUserService.GetUserName(),
                    userEmail = _externalAuthUserService.GetUserEmail(),
                    userRoles = _externalAuthUserService.GetUserRoles(),
                    isAuthenticated = _externalAuthUserService.IsAuthenticated()
                },
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetAttachment(
            [FromRoute] Guid uuidBuilding,
            [FromRoute] Guid uuidWorksheet,
            [FromRoute] Guid uuidRfq,
            Guid guid
        )
        {
            var attachment = await _context.Attachments
                .Where(
                    a => a.Guid == guid &&
                    a.BuildingGuid == uuidBuilding &&
                    a.WorkSheetGuid == uuidWorksheet &&
                    a.RfqGuid == uuidRfq
                )
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                return NotFound($"Nessun attachment trovato per il Worksheet con GUID '{uuidWorksheet}'.");
            }

            attachment.DeserializeComplexData();
            
            return Ok(new 
            {
                data = attachment,
                userInfo = new 
                {
                    userId = _externalAuthUserService.GetUserId(),
                    userName = _externalAuthUserService.GetUserName(),
                    userEmail = _externalAuthUserService.GetUserEmail(),
                    userRoles = _externalAuthUserService.GetUserRoles(),
                    isAuthenticated = _externalAuthUserService.IsAuthenticated()
                },
                timestamp = DateTime.UtcNow
            });
        }
    }
}