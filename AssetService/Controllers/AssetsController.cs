using AssetService.Clients;
using AssetService.Enums;
using AssetService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/[controller]")]
    [ApiController]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IAssetDataProviderClient _assetDataProviderClient;

        public AssetsController(
            IMappingServiceHttpClient mappingServiceHttpClient,
            IAssetDataProviderClient assetDataProviderClient
            )
        {
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _assetDataProviderClient = assetDataProviderClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssets()
        {
            var assets = await _assetDataProviderClient.GetAssetAsync();

            if (assets == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Impossibile recuperare i dati dei Asset dal servizio dati esterno.");
            }

            return Ok(assets);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AssetDto>> GetAsset(Guid guid)
        {
            var asset = await _assetDataProviderClient.GetAssetByIdAsync(guid);

            if (asset == null)
            {
                return NotFound($"Asset con GUID '{guid}' non trovato nel servizio dati esterno.");
            }

            return Ok(asset);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Asset>> CreateAsset([FromBody] Asset asset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mapping = await _mappingServiceHttpClient.GetMappingByGuidAsync(asset.Mapping);
            if (mapping == null)
            {
                return NotFound($"Mapping with ID {asset.Mapping} not found or inaccessible.");
            }
            // Validate Asset with mapping
            if (asset.UniqueIdentifiers == null)
            {
                asset.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            asset.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = asset.Guid.ToString()
            });

            asset.SerializeComplexData();

            var assetDtoToSend = new AssetDto
            {
                Guid = asset.Guid,
                Name = asset.Name,
                Mapping = mapping.Guid,
            };

            var createdAssetDto = await _assetDataProviderClient.CreateAssetAsync(assetDtoToSend);

            if (createdAssetDto == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante la creazione del Asset nel servizio dati esterno.");
            }


            asset.DeserializeComplexData();

            return CreatedAtAction(nameof(GetAsset), new { guid = asset.Guid }, asset);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsset(Guid guid, [FromBody] Asset asset)
        {
            if (guid != asset.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID dell'asset fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            asset.SerializeComplexData();

            var assetDtoToSend = new AssetDto
            {
                Guid = asset.Guid,
                Name = asset.Name,
                Mapping = asset.Mapping,
            };

            var success = await _assetDataProviderClient.UpdateAssetAsync(guid, assetDtoToSend);

            if (!success)
            {
                var existingAsset = await _assetDataProviderClient.GetAssetByIdAsync(guid);
                if (existingAsset == null)
                {
                    return NotFound($"Asset con GUID '{guid}' non trovato nel servizio dati esterno.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante l'aggiornamento dell'asset nel servizio dati esterno.");
            }

            return NoContent();
        }


        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsset(Guid guid)
        {
            var success = await _assetDataProviderClient.DeleteAssetAsync(guid);

            if (!success)
            {
                var existingAsset = await _assetDataProviderClient.GetAssetByIdAsync(guid);
                if (existingAsset == null)
                {
                    return NotFound($"Asset con GUID '{guid}' non trovato nel servizio dati esterno.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante l'eliminazione dell'asset nel servizio dati esterno.");
            }

            return NoContent();
        }
    }
}