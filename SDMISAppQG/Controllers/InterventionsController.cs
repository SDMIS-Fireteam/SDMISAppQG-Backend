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
public class InterventionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InterventionsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les interventions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InterventionEntity>>> GetInterventions()
    {
        return await _context.Interventions.ToListAsync();
    }

    /// <summary>
    /// Récupère une intervention par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InterventionEntity>> GetIntervention(Guid id)
    {
        var intervention = await _context.Interventions.FindAsync(id);

        if (intervention == null)
        {
            return NotFound();
        }

        return intervention;
    }

    /// <summary>
    /// Récupère toutes les interventions pour un incident spécifique
    /// </summary>
    [HttpGet("incident/{incidentId}")]
    public async Task<ActionResult<IEnumerable<InterventionEntity>>> GetInterventionsByIncident(Guid incidentId)
    {
        return await _context.Interventions
           .Where(i => i.IncidentId == incidentId)
           .ToListAsync();
    }

    /// <summary>
    /// Crée une nouvelle intervention
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InterventionEntity>> CreateIntervention(CreateInterventionDto dto)
    {
        var intervention = new InterventionEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IncidentId = dto.IncidentId,
            VehicleId = dto.VehicleId,
            Begin = dto.Begin,
            End = dto.End,
            Status = dto.Status,
            ConfirmedAt = dto.ConfirmedAt,
            ConfirmedBy = dto.ConfirmedBy
        };

        _context.Interventions.Add(intervention);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIntervention), new { id = intervention.Id }, intervention);
    }

    /// <summary>
    /// Met à jour une intervention existante
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIntervention(Guid id, UpdateInterventionDto dto)
    {
        var intervention = await _context.Interventions.FindAsync(id);
        if (intervention == null)
        {
            return NotFound();
        }

        if (dto.IncidentId.HasValue)
            intervention.IncidentId = dto.IncidentId.Value;
        if (dto.VehicleId.HasValue)
            intervention.VehicleId = dto.VehicleId.Value;
        if (dto.Begin.HasValue)
            intervention.Begin = dto.Begin.Value;
        if (dto.End.HasValue)
            intervention.End = dto.End.Value;
        if (dto.Status.HasValue)
            intervention.Status = dto.Status.Value;
        if (dto.ConfirmedAt.HasValue)
            intervention.ConfirmedAt = dto.ConfirmedAt.Value;
        if (dto.ConfirmedBy.HasValue)
            intervention.ConfirmedBy = dto.ConfirmedBy.Value;

        intervention.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await InterventionExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime une intervention
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIntervention(Guid id)
    {
        var intervention = await _context.Interventions.FindAsync(id);
        if (intervention == null)
        {
            return NotFound();
        }

        _context.Interventions.Remove(intervention);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Assigne un véhicule à un incident en créant une intervention
    /// </summary>
    [HttpPost("{incidentId}/createIntervention/{vehicleId}")]
    public async Task<IActionResult> AssignVehicleToIncident(Guid incidentId, Guid vehicleId)
    {
        var incident = await _context.Incidents.FindAsync(incidentId);
        if (incident == null)
        {
            return NotFound($"Incident with ID {incidentId} not found.");
        }

        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null)
        {
            return NotFound($"Vehicle with ID {vehicleId} not found.");
        }

        var intervention = new InterventionEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IncidentId = incidentId,
            VehicleId = vehicleId,
            Begin = DateTime.UtcNow,
            End = null,
            Status = Models.Enums.InterventionStatus.Pending,
            ConfirmedAt = null,
            ConfirmedBy = null,
        };

        _context.Interventions.Add(intervention);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> InterventionExists(Guid id)
    {
        return await _context.Interventions.AnyAsync(e => e.Id == id);
    }
}
