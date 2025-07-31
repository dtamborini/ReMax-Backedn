using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Models;
using AttachmentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;
using RemaxApi.Shared.Authentication.Services;

namespace AttachmentUserController.Controllers
{
    [Route("api/buildings/{uuidBuilding}/users/{uuidUser}/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentUserController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly IExternalAuthUserService _externalAuthUserService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;

        public AttachmentUserController(
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
            [FromRoute] Guid uuidUser
        )
        {
            var attachments = await _context.Attachments
                .Where(
                    a => a.BuildingGuid == uuidBuilding &&
                    a.UserGuid == uuidUser
                )
                .ToListAsync();

            if (!attachments.Any())
            {
                return NotFound($"Nessun attachment trovato per l'utente con GUID '{uuidUser}'.");
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
            [FromRoute] Guid uuidUser,
            Guid guid
        )
        {
            var attachment = await _context.Attachments
                .Where(
                    a => a.Guid == guid &&
                    a.BuildingGuid == uuidBuilding &&
                    a.UserGuid == uuidUser
                )
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                return NotFound($"Nessun attachment trovato con GUID '{guid}'.");
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