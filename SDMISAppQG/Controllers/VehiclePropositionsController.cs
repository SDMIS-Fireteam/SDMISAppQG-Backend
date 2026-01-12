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
public class VehiclePropositionsController : ControllerBase
{
    private readonly AppDbContext _context;
    public VehiclePropositionsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les propositions de véhicules
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehiclePropositionEntity>>> GetVehiclePropositions()
    {
        return await _context.VehiclePropositions
            .Include(p => p.Incident)
            .Include(p => p.Vehicle)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une proposition de véhicule par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehiclePropositionEntity>> GetVehicleProposition(Guid id)
    {
        var proposition = await _context.VehiclePropositions
            .Include(p => p.Incident)
            .Include(p => p.Vehicle)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (proposition == null)
        {
            return NotFound();
        }

        return proposition;
    }

    /// <summary>
    /// Récupère toutes les propositions pour une intervention spécifique
    /// </summary>
    [HttpGet("incident/{incidentId}")]
    public async Task<ActionResult<IEnumerable<VehiclePropositionEntity>>> GetPropositionsByIntervention(Guid incidentId)
    {
        var propositions = await _context.VehiclePropositions
            .Where(p => p.Incident.Id == incidentId)
            .OrderByDescending(p => p.Score)
            .Include(p => p.Vehicle)
            .Include(p => p.Incident)
            .Take(3)
            .ToListAsync();

        return propositions;
    }

    /// <summary>
    /// Récupère toutes les propositions pour un véhicule spécifique
    /// </summary>
    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<VehiclePropositionEntity>>> GetPropositionsByVehicle(Guid vehicleId)
    {
        var propositions = await _context.VehiclePropositions
            .Where(p => p.Vehicle.Id == vehicleId)
            .Include(p => p.Vehicle)
            .Include(p => p.Incident)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        return propositions;
    }

    /// <summary>
    /// Crée une nouvelle proposition de véhicule
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VehiclePropositionEntity>> CreateVehicleProposition(CreateVehiclePropositionDto dto)
    {
        // Vérifier que l'incident existe
        var incident = await _context.Incidents.FindAsync(dto.IncidentId);
        if (incident == null)
        {
            return BadRequest($"Incident with ID {dto.IncidentId} not found.");
        }

        // Vérifier que le véhicule existe
        var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
        if (vehicle == null)
        {
            return BadRequest($"Vehicle with ID {dto.VehicleId} not found.");
        }

        var proposition = new VehiclePropositionEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Incident = incident,
            Vehicle = vehicle,
            Score = dto.Score
        };

        _context.VehiclePropositions.Add(proposition);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehicleProposition), new { id = proposition.Id }, proposition);
    }

    /// <summary>
    /// Met à jour une proposition de véhicule existante
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicleProposition(Guid id, UpdateVehiclePropositionDto dto)
    {
        var proposition = await _context.VehiclePropositions.FindAsync(id);
        if (proposition == null)
        {
            return NotFound();
        }

        if (dto.IncidentId.HasValue)
        {
            var incident = await _context.Incidents.FindAsync(dto.IncidentId.Value);
            if (incident == null)
            {
                return BadRequest($"Incident with ID {dto.IncidentId.Value} not found.");
            }
            proposition.Incident = incident;
        }

        if (dto.VehicleId.HasValue)
        {
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId.Value);
            if (vehicle == null)
            {
                return BadRequest($"Vehicle with ID {dto.VehicleId.Value} not found.");
            }
            proposition.Vehicle = vehicle;
        }

        if (dto.Score.HasValue)
            proposition.Score = dto.Score.Value;

        proposition.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.VehiclePropositions.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime une proposition de véhicule
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicleProposition(Guid id)
    {
        var proposition = await _context.VehiclePropositions.FindAsync(id);
        if (proposition == null)
        {
            return NotFound();
        }

        _context.VehiclePropositions.Remove(proposition);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Supprime toutes les propositions pour une intervention spécifique
    /// </summary>
    [HttpDelete("incident/{incidentId}")]
    public async Task<IActionResult> DeletePropositionsByIncident(Guid incidentId)
    {
        var propositions = await _context.VehiclePropositions
            .Where(p => p.Incident.Id == incidentId)
            .ToListAsync();

        if (!propositions.Any())
        {
            return NotFound($"No propositions found for incident {incidentId}");
        }

        _context.VehiclePropositions.RemoveRange(propositions);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
