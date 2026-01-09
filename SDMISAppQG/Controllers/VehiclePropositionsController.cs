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
        return await _context.VehiclePropositions.ToListAsync();
    }

    /// <summary>
    /// Récupère une proposition de véhicule par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehiclePropositionEntity>> GetVehicleProposition(Guid id)
    {
        var proposition = await _context.VehiclePropositions.FindAsync(id);

        if (proposition == null)
        {
            return NotFound();
        }

        return proposition;
    }

    /// <summary>
    /// Récupère toutes les propositions pour une intervention spécifique
    /// </summary>
    [HttpGet("intervention/{interventionId}")]
    public async Task<ActionResult<IEnumerable<VehiclePropositionEntity>>> GetPropositionsByIntervention(Guid interventionId)
    {
        var propositions = await _context.VehiclePropositions
            .Where(p => p.InterventionId == interventionId)
            .OrderByDescending(p => p.Score)
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
            .Where(p => p.VehicleId == vehicleId)
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
        // Vérifier que l'intervention existe
        var interventionExists = await _context.Interventions.AnyAsync(i => i.Id == dto.InterventionId);
        if (!interventionExists)
        {
            return BadRequest($"Intervention with ID {dto.InterventionId} not found.");
        }

        // Vérifier que le véhicule existe
        var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId);
        if (!vehicleExists)
        {
            return BadRequest($"Vehicle with ID {dto.VehicleId} not found.");
        }

        var proposition = new VehiclePropositionEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            InterventionId = dto.InterventionId,
            VehicleId = dto.VehicleId,
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

        if (dto.InterventionId.HasValue)
        {
            var interventionExists = await _context.Interventions.AnyAsync(i => i.Id == dto.InterventionId.Value);
            if (!interventionExists)
            {
                return BadRequest($"Intervention with ID {dto.InterventionId.Value} not found.");
            }
            proposition.InterventionId = dto.InterventionId.Value;
        }

        if (dto.VehicleId.HasValue)
        {
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == dto.VehicleId.Value);
            if (!vehicleExists)
            {
                return BadRequest($"Vehicle with ID {dto.VehicleId.Value} not found.");
            }
            proposition.VehicleId = dto.VehicleId.Value;
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
    [HttpDelete("intervention/{interventionId}")]
    public async Task<IActionResult> DeletePropositionsByIntervention(Guid interventionId)
    {
        var propositions = await _context.VehiclePropositions
            .Where(p => p.InterventionId == interventionId)
            .ToListAsync();

        if (!propositions.Any())
        {
            return NotFound($"No propositions found for intervention {interventionId}");
        }

        _context.VehiclePropositions.RemoveRange(propositions);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
