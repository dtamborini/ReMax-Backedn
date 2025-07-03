using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;

namespace AttachmentWorkorderController.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/workorders/{uuidWorkorder}/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentWorkorderController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;

        public AttachmentWorkorderController(
            UserClaimService userClaimService,
            AttachmentDbContext context,
            IAttachmentFactoryService attachmentFactoryService,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _userClaimService = userClaimService;
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _attachmentFactoryService = attachmentFactoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Attachment>>> GetAttachments(
            [FromRoute] Guid uuidBuilding,
            [FromRoute] Guid uuidWorksheet,
            [FromRoute] Guid uuidWorkorder
            )
        {
            var attachments = await _context.Attachments
                .Where(
                    a => a.WorkSheetGuid == uuidWorksheet && 
                    a.BuildingGuid == uuidBuilding && 
                    a.WorkOrderGuid == uuidWorkorder
                )
                .ToListAsync();

            if (!attachments.Any())
            {
                return NotFound($"Nessun attachment trovato per il Worksheet con GUID '{uuidWorksheet}'.");
            }

            attachments.ForEach(b => b.DeserializeComplexData());
            return Ok(attachments);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Attachment>> GetAttachment(
            [FromRoute] Guid uuidBuilding,
            [FromRoute] Guid uuidWorksheet,
            [FromRoute] Guid uuidWorkorder,
            Guid guid
            )
        {
            var attachment = await _context.Attachments
                .Where(
                    a => a.Guid == guid &&
                    a.BuildingGuid == uuidBuilding &&
                    a.WorkSheetGuid == uuidWorksheet &&
                    a.WorkOrderGuid == uuidWorkorder
                )
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                return NotFound($"Nessun attachment trovato per il Worksheet con GUID '{uuidWorksheet}'.");
            }

            return Ok(attachment);
        }
    }
}