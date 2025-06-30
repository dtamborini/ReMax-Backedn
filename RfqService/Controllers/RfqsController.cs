using RfqService.Clients;
using RfqService.Data;
using RfqService.Enums;
using RfqService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RfqService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/[controller]")]
    [ApiController]
    [Authorize]
    public class RfqController : ControllerBase
    {
        private readonly RfqDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;

        public RfqController(
            RfqDbContext context,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Rfq>>> GetRfqs()
        {
            var rfqs = await _context.Rfqs.ToListAsync();

            rfqs.ForEach(b => b.DeserializeComplexData());
            return Ok(rfqs);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Rfq>> GetRfq(Guid guid)
        {
            var rfq = await _context.Rfqs.FindAsync(guid);

            if (rfq == null)
            {
                return NotFound($"RFQ con GUID '{guid}' non trovato.");
            }

            rfq.DeserializeComplexData();
            return Ok(rfq);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Rfq>> CreateRfq([FromBody] Rfq rfq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid? retrievedMappingGuid = await _mappingServiceHttpClient.GetMappingGuidByIdAsync(rfq.Mapping);
            if (retrievedMappingGuid == null)
            {
                return NotFound($"Mapping with ID {rfq.Mapping} not found or inaccessible.");
            }
            // Validate workOrder with mapping
            if (rfq.UniqueIdentifiers == null)
            {
                rfq.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            rfq.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = rfq.Guid.ToString()
            });

            rfq.SerializeComplexData();

            _context.Rfqs.Add(rfq);
            await _context.SaveChangesAsync();

            rfq.DeserializeComplexData();

            return CreatedAtAction(nameof(GetRfq), new { guid = rfq.Guid }, rfq);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRfq(Guid guid, [FromBody] Rfq rfq)
        {
            if (guid != rfq.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID del workOrder fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            rfq.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!rfqExists(guid))
                {
                    return NotFound($"RFQ con GUID '{guid}' non trovato.");
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
        public async Task<IActionResult> DeleteRfq(Guid guid)
        {
            var workOrder = await _context.Rfqs.FindAsync(guid);
            if (workOrder == null)
            {
                return NotFound($"WorkOrder con GUID '{guid}' non trovato.");
            }

            _context.Rfqs.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool rfqExists(Guid guid)
        {
            return _context.Rfqs.Any(e => e.Guid == guid);
        }
    }
}