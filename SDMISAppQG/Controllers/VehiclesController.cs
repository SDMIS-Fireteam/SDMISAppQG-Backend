using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase {
   private readonly AppDbContext _context;

   public VehiclesController(AppDbContext context) {
      _context = context;
   }

   /// <summary>
   /// Récupère tous les véhicules
   /// </summary>
   [HttpGet]
   public async Task<ActionResult<IEnumerable<VehicleEntity>>> GetVehicles() {
      return await _context.Vehicles.ToListAsync();
   }

   /// <summary>
   /// Récupère un véhicule par son ID
   /// </summary>
   [HttpGet("{id}")]
   public async Task<ActionResult<VehicleEntity>> GetVehicle(Guid id) {
      var vehicle = await _context.Vehicles.FindAsync(id);

      if (vehicle == null) {
         return NotFound();
      }

      return vehicle;
   }

   /// <summary>
   /// Crée un nouveau véhicule
   /// </summary>
   [HttpPost]
   public async Task<ActionResult<VehicleEntity>> CreateVehicle(VehicleEntity vehicle) {
      vehicle.Id = Guid.NewGuid();
      _context.Vehicles.Add(vehicle);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
   }

   /// <summary>
   /// Met à jour un véhicule existant
   /// </summary>
   [HttpPut("{id}")]
   public async Task<IActionResult> UpdateVehicle(Guid id, VehicleEntity vehicle) {
      if (id != vehicle.Id) {
         return BadRequest();
      }

      _context.Entry(vehicle).State = EntityState.Modified;

      try {
         await _context.SaveChangesAsync();
      } catch (DbUpdateConcurrencyException) {
         if (!await VehicleExists(id)) {
            return NotFound();
         }
         throw;
      }

      return NoContent();
   }

   /// <summary>
   /// Supprime un véhicule
   /// </summary>
   [HttpDelete("{id}")]
   public async Task<IActionResult> DeleteVehicle(Guid id) {
      var vehicle = await _context.Vehicles.FindAsync(id);
      if (vehicle == null) {
         return NotFound();
      }

      _context.Vehicles.Remove(vehicle);
      await _context.SaveChangesAsync();

      return NoContent();
   }

   private async Task<bool> VehicleExists(Guid id) {
      return await _context.Vehicles.AnyAsync(e => e.Id == id);
   }
}
