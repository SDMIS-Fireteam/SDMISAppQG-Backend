using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncidentTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public IncidentTypesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les types d'incidents
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidentTypeEntity>>> GetIncidentTypes()
    {
        return await _context.IncidentTypes.ToListAsync();
    }

    /// <summary>
    /// Récupère un type d'incident par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<IncidentTypeEntity>> GetIncidentType(Guid id)
    {
        var incidentType = await _context.IncidentTypes.FindAsync(id);

        if (incidentType == null)
        {
            return NotFound();
        }

        return incidentType;
    }

    /// <summary>
    /// Crée un nouveau type d'incident
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<IncidentTypeEntity>> CreateIncidentType(CreateIncidentTypeDto dto)
    {
        var incidentType = new IncidentTypeEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Label = dto.Label,
            Category = dto.Category
        };

        _context.IncidentTypes.Add(incidentType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncidentType), new { id = incidentType.Id }, incidentType);
    }

    /// <summary>
    /// Met à jour un type d'incident existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncidentType(Guid id, UpdateIncidentTypeDto dto)
    {
        var incidentType = await _context.IncidentTypes.FindAsync(id);
        if (incidentType == null)
        {
            return NotFound();
        }

        if (dto.Label != null)
            incidentType.Label = dto.Label;
        if (dto.Category != null)
            incidentType.Category = dto.Category;

        incidentType.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await IncidentTypeExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime un type d'incident
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncidentType(Guid id)
    {
        var incidentType = await _context.IncidentTypes.FindAsync(id);
        if (incidentType == null)
        {
            return NotFound();
        }

        _context.IncidentTypes.Remove(incidentType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> IncidentTypeExists(Guid id)
    {
        return await _context.IncidentTypes.AnyAsync(e => e.Id == id);
    }
}
