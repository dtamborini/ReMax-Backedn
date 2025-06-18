using BuildingService.Clients;
using BuildingService.Data;
using BuildingService.Enums;
using BuildingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuildingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BuildingsController : ControllerBase
    {
        private readonly BuildingDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;

        public BuildingsController(
            BuildingDbContext context,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            var buildings = await _context.Buildings.ToListAsync();

            buildings.ForEach(b => b.DeserializeComplexData());
            return Ok(buildings);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Building>> GetBuilding(Guid guid)
        {
            var building = await _context.Buildings.FindAsync(guid);

            if (building == null)
            {
                return NotFound($"Building con GUID '{guid}' non trovato.");
            }

            building.DeserializeComplexData();
            return Ok(building);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Building>> CreateBuilding([FromBody] Building building)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid? retrievedMappingGuid = await _mappingServiceHttpClient.GetMappingGuidByIdAsync(building.Mapping);
            if (retrievedMappingGuid == null)
            {
                return NotFound($"Mapping with ID {building.Mapping} not found or inaccessible.");
            }
            // Validate building with mapping
            if (building.UniqueIdentifiers == null)
            {
                building.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            building.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = building.Guid.ToString()
            });

            building.SerializeComplexData();

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            building.DeserializeComplexData();

            return CreatedAtAction(nameof(GetBuilding), new { guid = building.Guid }, building);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBuilding(Guid guid, [FromBody] Building building)
        {
            if (guid != building.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID del Building fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            building.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(guid))
                {
                    return NotFound($"Building con GUID '{guid}' non trovato.");
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
        public async Task<IActionResult> DeleteBuilding(Guid guid)
        {
            var building = await _context.Buildings.FindAsync(guid);
            if (building == null)
            {
                return NotFound($"Building con GUID '{guid}' non trovato.");
            }

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BuildingExists(Guid guid)
        {
            return _context.Buildings.Any(e => e.Guid == guid);
        }
    }
}