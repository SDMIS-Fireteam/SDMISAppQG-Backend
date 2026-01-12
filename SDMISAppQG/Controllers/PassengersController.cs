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
public class PassengersController : ControllerBase {
   private readonly AppDbContext _context;

   public PassengersController(AppDbContext context) {
      _context = context;
   }

   /// <summary>
   /// Retrieves all passengers for a specific vehicle.
   /// </summary>
   /// <param name="vehicleId">The unique identifier of the vehicle.</param>
   /// <returns>A list of PassengerDto.</returns>
   [HttpGet("vehicle/{vehicleId}")]
   public async Task<ActionResult<IEnumerable<PassengerDto>>> GetPassengersByVehicle(Guid vehicleId) {
      var passengers = await _context.Passengers
          .AsNoTracking()
          .Where(p => p.VehicleId == vehicleId)
          .Select(p => new PassengerDto {
             Id = p.Id,
             VehicleId = p.VehicleId,
             UserId = p.UserId,
             CreatedAt = p.CreatedAt
          })
          .ToListAsync();

      return Ok(passengers);
   }

   /// <summary>
   /// Assigns a user to a vehicle.
   /// </summary>
   /// <param name="dto">The passenger creation data.</param>
   /// <returns>The created PassengerDto.</returns>
   [HttpPost]
   public async Task<ActionResult<PassengerDto>> AssignPassenger(CreatePassengerDto dto) {
      var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
      if (vehicle == null) {
         return BadRequest($"Vehicle with ID {dto.VehicleId} not found.");
      }

      var user = await _context.Users.FindAsync(dto.UserId);
      if (user == null) {
         return BadRequest($"User with ID {dto.UserId} not found.");
      }

      if (user.Role != SDMISAppQG.Models.Enums.UserRole.Firefighter) {
         return BadRequest("Only users with the Firefighter role can be assigned to a vehicle.");
      }

      // Check if user is already in the vehicle (or any vehicle? Business rule unclear, assuming can only be in one place generally, but strictly duplicate in same vehicle is bad)
      var exists = await _context.Passengers.AnyAsync(p => p.VehicleId == dto.VehicleId && p.UserId == dto.UserId);
      if (exists) {
         return Conflict("User is already assigned to this vehicle.");
      }

      var passenger = new PassengerEntity {
         Id = Guid.NewGuid(),
         VehicleId = dto.VehicleId,
         UserId = dto.UserId,
         Vehicle = vehicle,
         User = user,
         CreatedAt = DateTime.UtcNow
      };

      _context.Passengers.Add(passenger);

      // Update Vehicle Passenger Count
      vehicle.PassengerCount = (vehicle.PassengerCount ?? 0) + 1;

      await _context.SaveChangesAsync();

      var resultDto = new PassengerDto {
         Id = passenger.Id,
         VehicleId = passenger.VehicleId,
         UserId = passenger.UserId,
         CreatedAt = passenger.CreatedAt
      };

      return CreatedAtAction(nameof(GetPassengersByVehicle), new { vehicleId = dto.VehicleId }, resultDto);
   }

   /// <summary>
   /// Removes a passenger from a vehicle.
   /// </summary>
   /// <param name="id">The unique identifier of the passenger record.</param>
   /// <returns>No content.</returns>
   [HttpDelete("{id}")]
   public async Task<IActionResult> RemovePassenger(Guid id) {
      var passenger = await _context.Passengers
          .Include(p => p.Vehicle)
          .FirstOrDefaultAsync(p => p.Id == id);

      if (passenger == null) {
         return NotFound();
      }

      var vehicle = passenger.Vehicle;

      _context.Passengers.Remove(passenger);

      // Update Vehicle Passenger Count
      if (vehicle != null) {
         vehicle.PassengerCount = Math.Max(0, (vehicle.PassengerCount ?? 0) - 1);
      }

      await _context.SaveChangesAsync();

      return NoContent();
   }
}
