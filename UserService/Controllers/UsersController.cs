using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Clients;
using UserService.Data;
using UserService.Enums;
using UserService.Models;
using UserService.Models.DTOs;
using UserService.Services;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(
            UserDbContext context, 
            IMappingServiceHttpClient mappingServiceHttpClient, 
            IPasswordHasher passwordHasher
        ){
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            users.ForEach(m => m.DeserializeComplexData());
            return users;
        }

        [HttpGet("{guid}")]
        public async Task<ActionResult<User>> GetUser(Guid guid)
        {
            var user = await _context.Users.FindAsync(guid);

            if (user == null)
            {
                return NotFound();
            }
            user.DeserializeComplexData();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] RegisterUserModel model)
        {

            var user = model.User;
            user.Username = model.Username;
            user.Guid = Guid.NewGuid();

            if (user.Mapping == Guid.Empty)
            {
                return BadRequest("MappingGuid cannot be empty.");
            }

            Guid? retrievedMappingGuid = await _mappingServiceHttpClient.GetMappingGuidByIdAsync(user.Mapping);

            if (retrievedMappingGuid == null)
            {
                return NotFound($"Mapping with ID {user.Mapping} not found or inaccessible.");
            }
            user.Mapping = retrievedMappingGuid.Value;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.HashPassword = _passwordHasher.HashPassword(model.Password);
            }
            else
            {
                return BadRequest("Password cannot be empty for new user.");
            }

            if (user.UniqueIdentifiers == null)
            {
                user.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            user.UniqueIdentifiers.Add(new EntityUniqueIdentifier{
                Type = UniqueIdentifierType.QR,
                Value = user.Guid.ToString()
            });

            user.SerializeComplexData();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.DeserializeComplexData();
            return CreatedAtAction("GetUser", new { guid = user.Guid }, user);
        }

        [HttpPut("{guid}")]
        public async Task<IActionResult> PutUser(Guid guid, User user)
        {
            if (guid != user.Guid)
            {
                return BadRequest();
            }

            user.SerializeComplexData();
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(guid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            user.DeserializeComplexData();
            return NoContent();
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteUser(Guid guid)
        {
            var user = await _context.Users.FindAsync(guid);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{guid}")]
        public async Task<IActionResult> PatchUser(Guid guid, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("La richiesta PATCH non contiene dati.");
            }

            var userToUpdate = await _context.Users.FindAsync(guid);

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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(guid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'applicazione della patch: {ex.Message}");
                return StatusCode(500, "Si è verificato un errore durante l'aggiornamento parziale dell'utente.");
            }

            userToUpdate.DeserializeComplexData();
            return Ok(userToUpdate);
        }

        private bool UserExists(Guid guid)
        {
            return _context.Users.Any(e => e.Guid == guid);
        }
    }
}