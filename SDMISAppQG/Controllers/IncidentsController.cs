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
        return await _context.Incidents.ToListAsync();
    }

    /// <summary>
    /// Récupère un incident par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<IncidentEntity>> GetIncident(Guid id)
    {
        var incident = await _context.Incidents
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
    public async Task<ActionResult<IncidentEntity>> CreateIncident(CreateIncidentDto dto)
    {
        var incidentType = await _context.IncidentTypes.FindAsync(dto.TypeId);
        if (incidentType == null)
        {
            return BadRequest($"Incident type with ID {dto.TypeId} not found.");
        }

        var incident = new IncidentEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            TypeId = incidentType.Id,
            Location = dto.Location,
            Severity = dto.Severity,
            Priority = dto.Priority,
            Status = dto.Status,
            Source = dto.Source,
            Description = dto.Description
        };

        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incident);
    }

    /// <summary>
    /// Met à jour un incident existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncident(Guid id, UpdateIncidentDto dto)
    {
        var incident = await _context.Incidents.FindAsync(id);
        if (incident == null)
        {
            return NotFound();
        }

        if (dto.TypeId != null)
        {
            var incidentType = await _context.IncidentTypes.FindAsync(dto.TypeId.Value);
            if (incidentType == null)
            {
                return BadRequest($"Incident type with ID {dto.TypeId.Value} not found.");
            }
            incident.TypeId = incidentType.Id;
        }

        if (dto.Location != null)
            incident.Location = dto.Location;
        if (dto.Severity.HasValue)
            incident.Severity = dto.Severity.Value;
        if (dto.Priority.HasValue)
            incident.Priority = dto.Priority.Value;
        if (dto.Status.HasValue)
            incident.Status = dto.Status.Value;
        if (dto.Source.HasValue)
            incident.Source = dto.Source.Value;
        if (dto.Description != null)
            incident.Description = dto.Description;

        incident.UpdatedAt = DateTime.UtcNow;

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
