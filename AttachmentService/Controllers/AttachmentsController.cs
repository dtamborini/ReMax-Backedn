using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Enums;
using AttachmentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;

namespace AttachmentService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;
        private readonly IEntityPropertyPatchService _entityPropertyPatchService;
        private readonly IAttachmentPatchService _attachmentPatchService;

        public AttachmentsController(
            UserClaimService userClaimService,
            AttachmentDbContext context,
            IAttachmentFactoryService attachmentFactoryService,
            IMappingServiceHttpClient mappingServiceHttpClient,
            IEntityPropertyPatchService entityPropertyPatchService,
            IAttachmentPatchService attachmentPatchService
            )
        {
            _context = context;
            _userClaimService = userClaimService;
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _attachmentFactoryService = attachmentFactoryService;
            _entityPropertyPatchService = entityPropertyPatchService;
            _attachmentPatchService = attachmentPatchService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Attachment>>> GetAttachments()
        {
            var attachments = await _context.Attachments.ToListAsync();

            attachments.ForEach(b => b.DeserializeComplexData());
            return Ok(attachments);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Attachment>> GetAttachment(Guid guid)
        {
            var attachment = await _context.Attachments.FindAsync(guid);

            if (attachment == null)
            {
                return NotFound($"Attachment con GUID '{guid}' non trovato.");
            }

            attachment.DeserializeComplexData();
            return Ok(attachment);
        }

        [HttpPatch("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAttachment(
            Guid uuidBuilding,
            Guid guid,
            [FromBody] Attachment patchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Guid == guid && a.BuildingGuid == uuidBuilding);

            if (attachment == null)
            {
                return NotFound($"Attachment con GUID '{guid}' non trovato per l'edificio '{uuidBuilding}'.");
            }

            Guid? userUuid = _userClaimService.GetUserGuidFromPrincipal(User);
            if (userUuid == null)
            {
                return Unauthorized("Impossibile determinare l'utente dal token di autenticazione o token non valido.");
            }

            var participation = new EntityParticipation
            {
                User = (Guid) userUuid,
                Timestamp = DateTime.UtcNow,
            };
            await _attachmentPatchService.ApplyAttachmentPatch(attachment, patchDto, participation);

            try
            {
                _context.Attachments.Update(attachment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!attachmentExists(guid))
                {
                    return NotFound($"Attachment con GUID '{guid}' non trovato.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il patch dell'allegato: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Si è verificato un errore durante l'aggiornamento dell'allegato: " + ex.Message);
            }

            return NoContent();
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAttachment(Guid guid, [FromBody] Attachment attachment)
        {
            if (guid != attachment.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID del workOrder fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            attachment.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!attachmentExists(guid))
                {
                    return NotFound($"Attachment con GUID '{guid}' non trovato.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAttachment(Guid guid)
        {
            var workOrder = await _context.Attachments.FindAsync(guid);
            if (workOrder == null)
            {
                return NotFound($"Attachment con GUID '{guid}' non trovato.");
            }

            _context.Attachments.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("tree")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttachmentTree>> GetAttachmentTree(Guid uuidBuilding)
        {
            var attachmentsForBuilding = await _context.Attachments
                .Where(a => a.BuildingGuid == uuidBuilding)
                .ToListAsync();

            if (!attachmentsForBuilding.Any())
            {
                return NotFound($"Nessun allegato trovato per l'edificio con UUID '{uuidBuilding}'.");
            }

            var attachmentTree = new AttachmentTree
            {
                BuildingId = uuidBuilding,
            };

            return Ok(attachmentTree);
        }

        [HttpGet("{guid}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAttachment(Guid uuidBuilding, Guid guid)
        {
            var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Guid == guid && a.BuildingGuid == uuidBuilding);

            if (attachment == null)
            {
                return NotFound($"Allegato con GUID '{guid}' non trovato per l'edificio '{uuidBuilding}'.");
            }

            // TODO: retrieve file logic.
            var filePath = Path.Combine("path/to/your/storage", attachment.Attachments[0].FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File '{attachment.Attachments[0].FileName}' non trovato sul server.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, attachment.Attachments[0].Content, attachment.Attachments[0].FileName);
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Attachment>> UploadAttachment(Guid uuidBuilding, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nessun file inviato.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachmentMapping = await _mappingServiceHttpClient.GetMappingsWithOptionalParameters(isActive: true, entityType: EntityType.Attachment);
            var entityMapping = attachmentMapping.FirstOrDefault();

            if (entityMapping == null)
            {
                return NotFound("Nessun 'Attachment' mapping attivo trovato. Impossibile procedere con l'upload.");
            }

            //var uploadPath = Path.Combine("path/to/your/storage", uuidBuilding.ToString());
            //if (!Directory.Exists(uploadPath))
            //{
            //    Directory.CreateDirectory(uploadPath);
            //

            var attachmentGuid = Guid.NewGuid();
            var fileName = file.FileName;
            var fileContentType = file.ContentType;
            //var filePath = Path.Combine(uploadPath, fileName);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            //User data
            Guid? userUuid = _userClaimService.GetUserGuidFromPrincipal(User);
            if (userUuid == null)
            {
                return Unauthorized("Impossibile determinare l'utente dal token di autenticazione o token non valido.");
            }

            // Build Entity
            var attachment = _attachmentFactoryService.CreateAttachment(
                attachmentGuid,
                fileName,
                entityMapping,
                uuidBuilding,
                userUuid.Value,
                fileContentType
            );

            attachment.SerializeComplexData();
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            attachment.DeserializeComplexData();

            return CreatedAtAction(nameof(GetAttachment), new { guid = attachment.Guid, uuidBuilding = uuidBuilding }, attachment);
        }

        [HttpPost("{guid}/upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Attachment>> UploadAttachmentWithGuid(Guid uuidBuilding, Guid guid, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nessun file inviato.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachmentMapping = await _mappingServiceHttpClient.GetMappingsWithOptionalParameters(isActive: true, entityType: EntityType.Attachment);
            var entityMapping = attachmentMapping.FirstOrDefault();

            if (entityMapping == null)
            {
                return NotFound("Nessun 'Attachment' mapping attivo trovato. Impossibile procedere con l'upload.");
            }

            //var uploadPath = Path.Combine("path/to/your/storage", uuidBuilding.ToString());
            //if (!Directory.Exists(uploadPath))
            //{
            //    Directory.CreateDirectory(uploadPath);
            //

            var fileName = file.FileName;
            var fileContentType = file.ContentType;
            //var filePath = Path.Combine(uploadPath, fileName);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            //User data
            Guid? userUuid = _userClaimService.GetUserGuidFromPrincipal(User);
            if (userUuid == null)
            {
                return Unauthorized("Impossibile determinare l'utente dal token di autenticazione o token non valido.");
            }

            // Build Entity
            var attachment = _attachmentFactoryService.CreateAttachment(
                guid,
                fileName,
                entityMapping,
                uuidBuilding,
                userUuid.Value,
                fileContentType
            );

            attachment.SerializeComplexData();
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            attachment.DeserializeComplexData();

            return CreatedAtAction(nameof(GetAttachment), new { guid = attachment.Guid, uuidBuilding = uuidBuilding }, attachment);
        }

        private bool attachmentExists(Guid guid)
        {
            return _context.Attachments.Any(e => e.Guid == guid);
        }
    }
}