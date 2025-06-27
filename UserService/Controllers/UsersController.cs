using UserService.Clients;
using UserService.Models.DTOs;
using UserService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Enums;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IUserDataProviderClient _userDataProviderClient;

        public UsersController(
            IMappingServiceHttpClient mappingServiceHttpClient,
            IUserDataProviderClient userDataProviderClient
        )
        {
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _userDataProviderClient = userDataProviderClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userDataProviderClient.GetUsersAsync();
            if (users == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Impossibile recuperare i dati degli utenti dal servizio dati esterno.");
            }

            return Ok(users);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUser(Guid guid)
        {
            var user = await _userDataProviderClient.GetUserByIdAsync(guid);

            if (user == null)
            {
                return NotFound($"User con GUID '{guid}' non trovato nel servizio dati esterno.");
            }

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid? retrievedMappingGuid = await _mappingServiceHttpClient.GetMappingGuidByIdAsync(user.Mapping);
            if (retrievedMappingGuid == null)
            {
                return NotFound($"Mapping with ID {user.Mapping} not found or inaccessible.");
            }

            if (user.UniqueIdentifiers == null)
            {
                user.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            user.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = user.Guid.ToString()
            });

            user.SerializeComplexData();

            var buildingDtoToSend = new UserDto
            {
                Guid = user.Guid,
                Name = user.Name,
                Mapping = (Guid)retrievedMappingGuid,
            };

            var createdBuildingDto = await _userDataProviderClient.CreateUserAsync(buildingDtoToSend);

            if (createdBuildingDto == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante la creazione del Building nel servizio dati esterno.");
            }


            user.DeserializeComplexData();

            return CreatedAtAction(nameof(GetUser), new { guid = user.Guid }, user);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(Guid guid, User user)
        {
            if (guid != user.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID dell'utente fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            user.SerializeComplexData();

            var buildingDtoToSend = new UserDto
            {
                Guid = user.Guid,
                Name = user.Name,
                Mapping = user.Mapping,
            };

            var success = await _userDataProviderClient.UpdateUserAsync(guid, buildingDtoToSend);

            if (!success)
            {
                var existingBuilding = await _userDataProviderClient.GetUserByIdAsync(guid);
                if (existingBuilding == null)
                {
                    return NotFound($"User con GUID '{guid}' non trovato nel servizio dati esterno.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Errore durante l'aggiornamento dell'utente nel servizio dati esterno.");
            }

            return NoContent();
        }

        [HttpPatch("{guid}")]
        public async Task<IActionResult> PatchUser(Guid guid, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("La richiesta PATCH non contiene dati.");
            }

            var userToUpdate = await _userDataProviderClient.GetUserByIdAsync(guid);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            userToUpdate.DeserializeComplexData();

            var updatedProperties = new Dictionary<string, object?>();

            if (user.Name != null)
            {
                userToUpdate.Name = user.Name;
                updatedProperties.Add(nameof(userToUpdate.Name), userToUpdate.Name);
            }

            if (user.UniqueIdentifiers != null)
            {
                userToUpdate.UniqueIdentifiers = user.UniqueIdentifiers;
                updatedProperties.Add(nameof(userToUpdate.UniqueIdentifiers), userToUpdate.UniqueIdentifiers);
            }

            if (user.States != null)
            {
                userToUpdate.States = user.States;
                updatedProperties.Add(nameof(userToUpdate.States), userToUpdate.States);
            }

            userToUpdate.SerializeComplexData();
            try
            {
                await _userDataProviderClient.UpdateUserAsync(userToUpdate.Guid, userToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'applicazione della patch: {ex.Message}");
                return StatusCode(500, "Si è verificato un errore durante l'aggiornamento parziale dell'utente.");
            }

            userToUpdate.DeserializeComplexData();
            return Ok(userToUpdate);
        }
    }
}