using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;

namespace AttachmentQuoteController.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/rfqs/{uuidRfq}/quotes/{uuidQuote}/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentQuoteController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;

        public AttachmentQuoteController(
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
            [FromRoute] Guid uuidRfq,
            [FromRoute] Guid uuidQuote
            )
        {
            var attachments = await _context.Attachments
                .Where(
                    a => a.BuildingGuid == uuidBuilding &&
                    a.WorkSheetGuid == uuidWorksheet &&
                    a.RfqGuid == uuidRfq &&
                    a.QuoteGuid == uuidQuote
                )
                .ToListAsync();

            if (!attachments.Any())
            {
                return NotFound($"Nessun attachment trovato per RFQ con GUID '{uuidWorksheet}'.");
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
            [FromRoute] Guid uuidRfq,
            [FromRoute] Guid uuidQuote,
            Guid guid
        )
        {
            var attachment = await _context.Attachments
                .Where(
                    a => a.Guid == guid &&
                    a.BuildingGuid == uuidBuilding &&
                    a.WorkSheetGuid == uuidWorksheet &&
                    a.RfqGuid == uuidRfq &&
                    a.QuoteGuid == uuidQuote
                )
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                return NotFound($"Attachment con GUID '{guid}' non trovato.");
            }

            attachment.DeserializeComplexData();
            return Ok(attachment);
        }
    }
}