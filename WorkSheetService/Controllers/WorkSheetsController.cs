using WorkSheetService.Clients;
using WorkSheetService.Data;
using WorkSheetService.Enums;
using WorkSheetService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorkSheetService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkSheetsController : ControllerBase
    {
        private readonly WorkSheetDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;

        public WorkSheetsController(
            WorkSheetDbContext context,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkSheet>>> GetworkSheets()
        {
            var workSheets = await _context.WorkSheets.ToListAsync();

            workSheets.ForEach(b => b.DeserializeComplexData());
            return Ok(workSheets);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkSheet>> GetworkSheet(Guid guid)
        {
            var workSheet = await _context.WorkSheets.FindAsync(guid);

            if (workSheet == null)
            {
                return NotFound($"WorkSheet con GUID '{guid}' non trovats.");
            }

            workSheet.DeserializeComplexData();
            return Ok(workSheet);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkSheet>> CreateworkSheet([FromBody] WorkSheet workSheet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mapping = await _mappingServiceHttpClient.GetMappingByGuidAsync(workSheet.Mapping);
            if (mapping == null)
            {
                return NotFound($"Mapping with ID {workSheet.Mapping} not found or inaccessible.");
            }

            // Validate workSheet with mapping
            if (workSheet.UniqueIdentifiers == null)
            {
                workSheet.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            workSheet.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = workSheet.Guid.ToString()
            });

            workSheet.SerializeComplexData();

            _context.WorkSheets.Add(workSheet);
            await _context.SaveChangesAsync();

            workSheet.DeserializeComplexData();

            return CreatedAtAction(nameof(GetworkSheet), new { guid = workSheet.Guid }, workSheet);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateworkSheet(Guid guid, [FromBody] WorkSheet workSheet)
        {
            if (guid != workSheet.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID del workSheet fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            workSheet.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!workSheetExists(guid))
                {
                    return NotFound($"WorkSheet con GUID '{guid}' non trovato.");
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
        public async Task<IActionResult> DeleteworkSheet(Guid guid)
        {
            var workSheet = await _context.WorkSheets.FindAsync(guid);
            if (workSheet == null)
            {
                return NotFound($"WorkSheet con GUID '{guid}' non trovato.");
            }

            _context.WorkSheets.Remove(workSheet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool workSheetExists(Guid guid)
        {
            return _context.WorkSheets.Any(e => e.Guid == guid);
        }
    }
}