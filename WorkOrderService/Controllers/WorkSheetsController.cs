using WorkOrderService.Clients;
using WorkOrderService.Data;
using WorkOrderService.Enums;
using WorkOrderService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorkOrderService.Controllers
{
    [Route("api/buildings/{uuidBuilding}/worksheets/{uuidWorksheet}/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkOrderController : ControllerBase
    {
        private readonly WorkOrderDbContext _context;
        private readonly IMappingServiceHttpClient _mappingServiceHttpClient;

        public WorkOrderController(
            WorkOrderDbContext context,
            IMappingServiceHttpClient mappingServiceHttpClient
            )
        {
            _context = context;
            _mappingServiceHttpClient = mappingServiceHttpClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders()
        {
            var workOrders = await _context.WorkOrders.ToListAsync();

            workOrders.ForEach(b => b.DeserializeComplexData());
            return Ok(workOrders);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkOrder>> GetWorkOrder(Guid guid)
        {
            var workOrdes = await _context.WorkOrders.FindAsync(guid);

            if (workOrdes == null)
            {
                return NotFound($"WorkOrder con GUID '{guid}' non trovato.");
            }

            workOrdes.DeserializeComplexData();
            return Ok(workOrdes);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WorkOrder>> CreateWorkOrder([FromBody] WorkOrder workOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mapping = await _mappingServiceHttpClient.GetMappingByGuidAsync(workOrder.Mapping);
            if (mapping == null)
            {
                return NotFound($"Mapping with ID {workOrder.Mapping} not found or inaccessible.");
            }
            // Validate workOrder with mapping
            if (workOrder.UniqueIdentifiers == null)
            {
                workOrder.UniqueIdentifiers = new List<EntityUniqueIdentifier>();
            }

            workOrder.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = workOrder.Guid.ToString()
            });

            workOrder.SerializeComplexData();

            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();

            workOrder.DeserializeComplexData();

            return CreatedAtAction(nameof(GetWorkOrder), new { guid = workOrder.Guid }, workOrder);
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWorkOrder(Guid guid, [FromBody] WorkOrder workOrder)
        {
            if (guid != workOrder.Guid)
            {
                return BadRequest("Il GUID nella rotta non corrisponde al GUID del workOrder fornito.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            workOrder.SerializeComplexData();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!workOrderExists(guid))
                {
                    return NotFound($"WorkOrder con GUID '{guid}' non trovato.");
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
            var workOrder = await _context.WorkOrders.FindAsync(guid);
            if (workOrder == null)
            {
                return NotFound($"WorkOrder con GUID '{guid}' non trovato.");
            }

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool workOrderExists(Guid guid)
        {
            return _context.WorkOrders.Any(e => e.Guid == guid);
        }
    }
}