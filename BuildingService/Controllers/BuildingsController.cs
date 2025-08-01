using BuildingService.Clients;
using BuildingService.Enums;
using BuildingService.Interfaces;
using BuildingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaxApi.Shared.Authentication.Services;

namespace BuildingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BuildingsController : ControllerBase
    {
        private readonly UserClaimService _userClaimService;
        private readonly IBuildingFactoryService _buildingFactoryService;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IBuildingDataProviderClient _buildingDataProviderClient;
        private readonly IExternalAuthUserService _userService;

        public BuildingsController(
            UserClaimService userClaimService,
            IBuildingFactoryService buildingFactoryService,
            IMappingServiceHttpClient mappingServiceHttpClient,
            IBuildingDataProviderClient buildingDataProviderClient,
            IExternalAuthUserService userService
            )
        {
            _userClaimService = userClaimService;
            _buildingFactoryService = buildingFactoryService;
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _buildingDataProviderClient = buildingDataProviderClient;
            _userService = userService;
            
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Building>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDao))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDao))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDao))]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            var buildingDtos = await _buildingDataProviderClient.GetBuildingsAsync();

            if (buildingDtos == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                {
                    new LocalizedTitle
                    {
                        Culture = "en-US",
                        Value = "No building data available from external provider or data could not be retrieved."
                    },
                    new LocalizedTitle
                    {
                        Culture = "it-IT",
                        Value = "Nessun dato di edificio disponibile dal provider esterno o i dati non sono stati recuperati."
                    },
                }
                };
                return BadRequest(errorResponse);
            }

            //Mapping
            var buildingMappings = await _mappingServiceHttpClient.GetMappingsWithOptionalParameters(isActive: true, entityType: EntityType.Building);
            var entityMapping = buildingMappings.FirstOrDefault();

            if (entityMapping == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                {
                    new LocalizedTitle { Culture = "en-US", Value = "No active 'Building' mapping found. Cannot process data." },
                    new LocalizedTitle { Culture = "it-IT", Value = "Nessun mapping 'Building' attivo trovato. Impossibile processare i dati." }
                }
                };
                return NotFound(errorResponse);
            }

            //User
            Guid? userUuid = _userClaimService.GetUserGuidFromPrincipal(User);
            if (userUuid == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                {
                    new LocalizedTitle { Culture = "en-US", Value = "Unable to determine user from authentication token or invalid token." },
                    new LocalizedTitle { Culture = "it-IT", Value = "Impossibile determinare l'utente dal token di autenticazione o token non valido." }
                }
                };
                return Unauthorized(errorResponse);
            }

            // --- Mappatura di ogni BuildingDto nella lista ---
            var mappedBuildings = new List<Building>();
            foreach (var buildingDto in buildingDtos)
            {
                try
                {
                    var mappedBuilding = _buildingFactoryService.MapBuildingDto(
                        buildingDto, entityMapping, (Guid)userUuid
                    );
                    mappedBuildings.Add(mappedBuilding);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante la mappatura di un edificio: {ex.Message}");
                }
            }

            if (!mappedBuildings.Any() && buildingDtos.Any())
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                {
                    new LocalizedTitle { Culture = "en-US", Value = "Failed to map any building data after retrieval." },
                    new LocalizedTitle { Culture = "it-IT", Value = "Nessun dato di edificio mappato con successo dopo il recupero." }
                }
                };
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }

            return Ok(new 
            {
                data = mappedBuildings,
                userInfo = new 
                {
                    userId = _userService.GetUserId(),
                    userName = _userService.GetUserName(),
                    userEmail = _userService.GetUserEmail(),
                    userRoles = _userService.GetUserRoles(),
                    isAuthenticated = _userService.IsAuthenticated()
                },
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("v2")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Building>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDao))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDao))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDao))]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildingsV2()
        {
            // Extract user ID from JWT token
            var userIdString = _userClaimService.GetUserIdFromJwt(User);
            if (string.IsNullOrEmpty(userIdString))
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                    {
                        new LocalizedTitle { Culture = "en-US", Value = "Unable to determine user ID from JWT token." },
                        new LocalizedTitle { Culture = "it-IT", Value = "Impossibile determinare l'ID utente dal token JWT." }
                    }
                };
                return Unauthorized(errorResponse);
            }

            // Try to use user-filtered client if available
            IEnumerable<BuildingDto>? buildingDtos;
            
            if (_buildingDataProviderClient is IUserFilteredBuildingDataProviderClient userFilteredClient)
            {
                // Use the enhanced client that supports user filtering
                buildingDtos = await userFilteredClient.GetBuildingsForUserAsync(userIdString);
            }
            else
            {
                // Fallback to getting all buildings and filter manually
                var allBuildings = await _buildingDataProviderClient.GetBuildingsAsync();
                buildingDtos = allBuildings;
                /*buildingDtos = allBuildings?.Where(b =>
                    // Simulate user filtering based on building properties
                    b.Name != null && b.Name.Contains(userIdString.Substring(0, Math.Min(4, userIdString.Length)))
                );*/
            }

            if (buildingDtos == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                    {
                        new LocalizedTitle
                        {
                            Culture = "en-US",
                            Value = "No building data available from external provider or data could not be retrieved."
                        },
                        new LocalizedTitle
                        {
                            Culture = "it-IT",
                            Value = "Nessun dato di edificio disponibile dal provider esterno o i dati non sono stati recuperati."
                        },
                    }
                };
                return BadRequest(errorResponse);
            }

            var userFilteredBuildings = buildingDtos.ToList();

            // Get mapping configuration
            var buildingMappings = await _mappingServiceHttpClient.GetMappingsWithOptionalParameters(isActive: true, entityType: EntityType.Building);
            var entityMapping = buildingMappings.FirstOrDefault();

            if (entityMapping == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                    {
                        new LocalizedTitle { Culture = "en-US", Value = "No active 'Building' mapping found. Cannot process data." },
                        new LocalizedTitle { Culture = "it-IT", Value = "Nessun mapping 'Building' attivo trovato. Impossibile processare i dati." }
                    }
                };
                return NotFound(errorResponse);
            }

            // Get user GUID for mapping
            var userGuid = _userClaimService.GetUserGuidFromJwt(User);
            if (userGuid == null)
            {
                // Fallback: try to parse user ID as GUID or create a deterministic GUID
                if (Guid.TryParse(userIdString, out Guid parsedGuid))
                {
                    userGuid = parsedGuid;
                }
                else
                {
                    // Create a deterministic GUID from user ID string
                    userGuid = Guid.NewGuid(); // In production, use a deterministic method
                }
            }

            // Map filtered buildings
            var mappedBuildings = new List<Building>();
            foreach (var buildingDto in userFilteredBuildings)
            {
                try
                {
                    var mappedBuilding = _buildingFactoryService.MapBuildingDto(
                        buildingDto, entityMapping, userGuid.Value
                    );
                    mappedBuildings.Add(mappedBuilding);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante la mappatura di un edificio: {ex.Message}");
                }
            }

            return Ok(new 
            {
                data = mappedBuildings,
               /* userInfo = new 
                {
                    userId = userIdString,
                    userName = _userService.GetUserName(),
                    userEmail = _userService.GetUserEmail(),
                    userRoles = _userService.GetUserRoles(),
                    isAuthenticated = _userService.IsAuthenticated(),
                    extractedFromJwt = true
                },
                filterInfo = new
                {
                    totalBuildingsFromProvider = buildingDtos.Count(),
                    filteredBuildingsForUser = userFilteredBuildings.Count,
                    mappedSuccessfully = mappedBuildings.Count
                },
                timestamp = DateTime.UtcNow*/
            });
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDao))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDao))]
        public async Task<ActionResult<Building>> GetBuilding(Guid guid)
        {
            var buildingDto = await _buildingDataProviderClient.GetBuildingByIdAsync(guid);

            if (buildingDto == null)
            {
                var errorResponse = new ErrorDao
                {
                    Titles = new List<LocalizedTitle>
                    {
                        new LocalizedTitle
                        {
                            Culture = "en-US",
                            Value = $"Cannot find a valid Building with GUID:'{guid}' in external provider."
                        },
                        new LocalizedTitle
                        {
                            Culture = "it-IT",
                            Value = $"Building con GUID:'{guid}' non trovato nel servizio dati esterno"
                        },
                    }
                };
                return NotFound(errorResponse);
            }

            //Mapping
            var attachmentMapping = await _mappingServiceHttpClient.GetMappingsWithOptionalParameters(isActive: true, entityType: EntityType.Building);
            var entityMapping = attachmentMapping.FirstOrDefault();

            if (entityMapping == null)
            {
                return NotFound("Nessun 'Attachment' mapping attivo trovato. Impossibile procedere con l'upload.");
            }

            //User
            Guid? userUuid = _userClaimService.GetUserGuidFromPrincipal(User);
            if (userUuid == null)
            {
                return Unauthorized("Impossibile determinare l'utente dal token di autenticazione o token non valido.");
            }

            var building = _buildingFactoryService.MapBuildingDto(
                buildingDto, entityMapping, (Guid) userUuid

            );

            return Ok(new 
            {
                data = building,
                userInfo = new 
                {
                    userId = _userService.GetUserId(),
                    userName = _userService.GetUserName(),
                    userEmail = _userService.GetUserEmail(),
                    userRoles = _userService.GetUserRoles(),
                    isAuthenticated = _userService.IsAuthenticated()
                },
                timestamp = DateTime.UtcNow
            });
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

            var mapping = await _mappingServiceHttpClient.GetMappingByGuidAsync(building.Mapping);
            if (mapping == null)
            {
                return NotFound($"Mapping with ID {building.Mapping} not found or inaccessible.");
            }
            if (mapping == null)
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

            return CreatedAtAction(nameof(GetBuilding), new { guid = building.Guid }, new 
            {
                data = building,
                userInfo = new 
                {
                    userId = _userService.GetUserId(),
                    userName = _userService.GetUserName(),
                    userEmail = _userService.GetUserEmail(),
                    userRoles = _userService.GetUserRoles(),
                    isAuthenticated = _userService.IsAuthenticated()
                },
                timestamp = DateTime.UtcNow
            });
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
                Address = "Address",
                TelephoneNumber = "TelephoneNumber",
                Emails = new BuildingEmailDto
                {
                    Email = "Email",
                    Pec = "P.E.C.",
                },
                Cf = "CF",
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