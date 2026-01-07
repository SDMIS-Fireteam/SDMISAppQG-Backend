using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public IncidentsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les incidents
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidentEntity>>> GetIncidents()
    {
        return await _context.Incidents.Include(i => i.Type).ToListAsync();
    }

    /// <summary>
    /// Récupère un incident par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<IncidentEntity>> GetIncident(Guid id)
    {
        var incident = await _context.Incidents
           .Include(i => i.Type)
           .FirstOrDefaultAsync(i => i.Id == id);

        if (incident == null)
        {
            return NotFound();
        }

        return incident;
    }

    /// <summary>
    /// Crée un nouveau incident
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IncidentEntity>> CreateIncident(IncidentEntity incident)
    {
        incident.Id = Guid.NewGuid();
        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
    }

    /// <summary>
    /// Met à jour un incident existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncident(Guid id, IncidentEntity incident)
    {
        if (id != incident.Id)
        {
            return BadRequest();
        }

        _context.Entry(incident).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await IncidentExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime un incident
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncident(Guid id)
    {
        var incident = await _context.Incidents.FindAsync(id);
        if (incident == null)
        {
            return NotFound();
        }

        _context.Incidents.Remove(incident);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> IncidentExists(Guid id)
    {
        return await _context.Incidents.AnyAsync(e => e.Id == id);
    }
}
