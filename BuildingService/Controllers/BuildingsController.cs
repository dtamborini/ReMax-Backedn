using BuildingService.Clients;
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
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IBuildingDataProviderClient _buildingDataProviderClient;

        public BuildingsController(
            IMappingServiceHttpClient mappingServiceHttpClient,
            IBuildingDataProviderClient buildingDataProviderClient
            )
        {
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _buildingDataProviderClient = buildingDataProviderClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BuildingDto>>> GetBuildings()
        {
            var buildings = await _buildingDataProviderClient.GetBuildingsAsync();

            if (buildings == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Impossibile recuperare i dati dei Building dal servizio dati esterno.");
            }

            return Ok(buildings);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BuildingDto>> GetBuilding(Guid guid)
        {
            var building = await _buildingDataProviderClient.GetBuildingByIdAsync(guid);

            if (building == null)
            {
                return NotFound($"Building con GUID '{guid}' non trovato nel servizio dati esterno.");
            }

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

            var buildingDtoToSend = new BuildingDto
            {
                Guid = building.Guid,
                Name = building.Name,
                Mapping = (Guid) retrievedMappingGuid,
            };

            var createdBuildingDto = await _buildingDataProviderClient.CreateBuildingAsync(buildingDtoToSend);

            if (createdBuildingDto == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante la creazione del Building nel servizio dati esterno.");
            }


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

            var buildingDtoToSend = new BuildingDto
            {
                Guid = building.Guid,
                Name = building.Name,
                Mapping = building.Mapping,
            };

            var success = await _buildingDataProviderClient.UpdateBuildingAsync(guid, buildingDtoToSend);

            if (!success)
            {
                var existingBuilding = await _buildingDataProviderClient.GetBuildingByIdAsync(guid);
                if (existingBuilding == null)
                {
                    return NotFound($"Building con GUID '{guid}' non trovato nel servizio dati esterno.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante l'aggiornamento del Building nel servizio dati esterno.");
            }

            return NoContent();
        }


        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBuilding(Guid guid)
        {
            var success = await _buildingDataProviderClient.DeleteBuildingAsync(guid);

            if (!success)
            {
                var existingBuilding = await _buildingDataProviderClient.GetBuildingByIdAsync(guid);
                if (existingBuilding == null)
                {
                    return NotFound($"Building con GUID '{guid}' non trovato nel servizio dati esterno.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante l'eliminazione del Building nel servizio dati esterno.");
            }

            return NoContent();
        }
    }
}