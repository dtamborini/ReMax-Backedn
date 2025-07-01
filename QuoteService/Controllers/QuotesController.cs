using QuoteService.Clients;
using QuoteService.Data;
using QuoteService.Enums;
using QuoteService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QuoteService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/rfqs/{uuidRfq}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;

        public QuotesController(
            QuoteDbContext context,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
        {
            var workSheets = await _context.Quotes.ToListAsync();

            workSheets.ForEach(b => b.DeserializeComplexData());
            return Ok(workSheets);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Quote>> GetQuote(Guid guid)
        {
            var quote = await _context.Quotes.FindAsync(guid);

            if (quote == null)
            {
                return NotFound($"Quote con GUID '{guid}' non trovats.");
            }

            quote.DeserializeComplexData();
            return Ok(quote);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Quote>> CreateQuote([FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mapping = await _mappingServiceHttpClient.GetMappingByGuidAsync(quote.Mapping);
            if (mapping == null)
            {
                return NotFound($"Mapping with ID {quote.Mapping} not found or inaccessible.");
            }
            // Validate workSheet with mapping
            if (quote.UniqueIdentifiers == null)
            {
                quote.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            quote.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = quote.Guid.ToString()
            });

            quote.SerializeComplexData();

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            quote.DeserializeComplexData();

            return CreatedAtAction(nameof(GetQuote), new { guid = quote.Guid }, quote);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateworkSheet(Guid guid, [FromBody] Quote quote)
        {
            if (guid != quote.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID della richiesta di preventivo fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            quote.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!quoteExist(guid))
                {
                    return NotFound($"Quote con GUID '{guid}' non trovato.");
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
        public async Task<IActionResult> DeleteQuote(Guid guid)
        {
            var workSheet = await _context.Quotes.FindAsync(guid);
            if (workSheet == null)
            {
                return NotFound($"Quote con GUID '{guid}' non trovato.");
            }

            _context.Quotes.Remove(workSheet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool quoteExist(Guid guid)
        {
            return _context.Quotes.Any(e => e.Guid == guid);
        }
    }
}