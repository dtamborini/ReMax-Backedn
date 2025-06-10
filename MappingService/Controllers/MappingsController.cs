using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MappingService.Data;
using MappingService.Models;

namespace MappingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MappingsController : ControllerBase
    {
        private readonly MappingDbContext _context;

        public MappingsController(MappingDbContext context)
        {
            _context = context;
        }

        // GET: api/Mappings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityMapping>>> GetMappings()
        {
            return await _context.Mappings.ToListAsync();
        }

        // POST: api/Mappings
        [HttpPost]
        public async Task<ActionResult<EntityMapping>> PostMappingEntry(EntityMapping mapping)
        {
            _context.Mappings.Add(mapping);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMappings", new { guid = mapping.Guid }, mapping);
        }

    }
}