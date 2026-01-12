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
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehiclesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les véhicules
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleEntity>>> GetVehicles()
    {
        return await _context.Vehicles.Include(v => v.Type).ToListAsync();
    }

    /// <summary>
    /// Récupère un véhicule par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleEntity>> GetVehicle(Guid id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Type)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    /// <summary>
    /// Crée un nouveau véhicule
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VehicleEntity>> CreateVehicle(CreateVehicleDto dto)
    {
        if (await _context.Vehicles.AnyAsync(v => v.IdHardware == dto.IdHardware))
        {
            return BadRequest($"Vehicle with IdHardware {dto.IdHardware} already exists.");
        }

        var vehicleType = await _context.VehicleTypes.FindAsync(dto.TypeId);
        if (vehicleType == null)
        {
            return BadRequest($"Vehicle type with ID {dto.TypeId} not found.");
        }

        var vehicle = new VehicleEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IdHardware = dto.IdHardware,
            Type = vehicleType,
            LastLocation = dto.LastLocation,
            Availability = dto.Availability,
            UnavailabilityReason = dto.UnavailabilityReason,
            Fuel = dto.Fuel,
            Consumable = dto.Consumable,
            PassengerCount = dto.PassengerCount
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// Met à jour un véhicule existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, UpdateVehicleDto dto)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }



        if (dto.IdHardware.HasValue)
            vehicle.IdHardware = dto.IdHardware.Value;
        if (dto.TypeId.HasValue)
        {
            var vehicleType = await _context.VehicleTypes.FindAsync(dto.TypeId.Value);
            if (vehicleType == null)
            {
                return BadRequest($"Vehicle type with ID {dto.TypeId.Value} not found.");
            }
            vehicle.Type = vehicleType;
        }
        if (dto.LastLocation != null)
            vehicle.LastLocation = dto.LastLocation;
        if (dto.Availability.HasValue)
            vehicle.Availability = dto.Availability.Value;
        if (dto.UnavailabilityReason.HasValue)
            vehicle.UnavailabilityReason = dto.UnavailabilityReason.Value;
        if (dto.Fuel.HasValue)
            vehicle.Fuel = dto.Fuel.Value;
        if (dto.Consumable != null)
            vehicle.Consumable = dto.Consumable;
        if (dto.PassengerCount.HasValue)
            vehicle.PassengerCount = dto.PassengerCount.Value;

        vehicle.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Vehicles.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime un véhicule
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
