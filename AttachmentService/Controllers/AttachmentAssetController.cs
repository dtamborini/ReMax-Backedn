﻿using AttachmentService.Clients;
using AttachmentService.Data;
using AttachmentService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttachmentService.Interfaces;

namespace AttachmentWorksheetController.Controllers
{
    [Route("api/buildings/{uuidBuilding}/assets/{uuidAsset}/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentAssetService : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly AttachmentDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAttachmentFactoryService _attachmentFactoryService;

        public AttachmentAssetService(
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
            [FromRoute] Guid uuidAsset
        )
        {
            var attachments = await _context.Attachments
                .Where(
                    a => a.BuildingGuid == uuidBuilding &&
                    a.AssetGuid == uuidAsset
                )
                .ToListAsync();

            attachments.ForEach(b => b.DeserializeComplexData());
            return Ok(attachments);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Attachment>> GetAttachment(
            [FromRoute] Guid uuidBuilding,
            [FromRoute] Guid uuidAsset,
            Guid guid
        )
        {
            var attachment = await _context.Attachments
                .Where(
                    a => a.Guid == guid &&
                    a.BuildingGuid == uuidBuilding &&
                    a.AssetGuid == uuidAsset
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