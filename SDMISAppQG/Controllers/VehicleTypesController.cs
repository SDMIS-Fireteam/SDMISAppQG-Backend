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
public class VehicleTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehicleTypesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les types de véhicules
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleTypeEntity>>> GetVehicleTypes()
    {
        return await _context.VehicleTypes.ToListAsync();
    }

    /// <summary>
    /// Récupère un type de véhicule par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleTypeEntity>> GetVehicleType(Guid id)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);

        if (vehicleType == null)
        {
            return NotFound();
        }

        return vehicleType;
    }

    /// <summary>
    /// Crée un nouveau type de véhicule
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VehicleTypeEntity>> CreateVehicleType(CreateVehicleTypeDto dto)
    {
        var vehicleType = new VehicleTypeEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Label = dto.Label,
            CrewCapacity = dto.CrewCapacity
        };

        _context.VehicleTypes.Add(vehicleType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehicleType), new { id = vehicleType.Id }, vehicleType);
    }

    /// <summary>
    /// Met à jour un type de véhicule existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicleType(Guid id, UpdateVehicleTypeDto dto)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType == null)
        {
            return NotFound();
        }

        if (dto.Label != null)
            vehicleType.Label = dto.Label;
        if (dto.CrewCapacity.HasValue)
            vehicleType.CrewCapacity = dto.CrewCapacity.Value;

        vehicleType.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await VehicleTypeExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime un type de véhicule
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicleType(Guid id)
    {
        var vehicleType = await _context.VehicleTypes.FindAsync(id);
        if (vehicleType == null)
        {
            return NotFound();
        }

        _context.VehicleTypes.Remove(vehicleType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> VehicleTypeExists(Guid id)
    {
        return await _context.VehicleTypes.AnyAsync(e => e.Id == id);
    }
}
