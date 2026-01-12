using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AssigneesController : ControllerBase {
   private readonly AppDbContext _context;
   private readonly InterventionService _interventionService;

   public AssigneesController(AppDbContext context, InterventionService interventionService) {
      _context = context;
      _interventionService = interventionService;
   }

   /// <summary>
   /// Récupère toutes les assignations
   /// </summary>
   [HttpGet]
   public async Task<ActionResult<IEnumerable<Assigned>>> GetAssignees() {
      return await _context.Assignees.ToListAsync();
   }

   /// <summary>
   /// Récupère une assignation par son ID
   /// </summary>
   [HttpGet("{id}")]
   public async Task<ActionResult<Assigned>> GetAssignee(Guid id) {
      var assignee = await _context.Assignees
         .FirstOrDefaultAsync(a => a.Id == id);

      if (assignee == null) {
         return NotFound();
      }

      return assignee;
   }

   /// <summary>
   /// Récupère toutes les assignations d'une intervention
   /// </summary>
   [HttpGet("intervention/{interventionId}")]
   public async Task<ActionResult<IEnumerable<Assigned>>> GetAssigneesByIntervention(Guid interventionId) {
      return await _context.Assignees
          .Where(a => a.InterventionId == interventionId)
          .ToListAsync();
   }

   /// <summary>
   /// Récupère toutes les assignations d'un véhicule
   /// </summary>
   [HttpGet("vehicle/{vehicleId}")]
   public async Task<ActionResult<IEnumerable<Assigned>>> GetAssigneesByVehicle(Guid vehicleId) {
      return await _context.Assignees
          .Where(a => a.VehicleId == vehicleId)
          .ToListAsync();
   }

   /// <summary>
   /// Crée une nouvelle assignation
   /// </summary>
   [HttpPost]
   public async Task<ActionResult<Assigned>> CreateAssignee(CreateAssignedDto dto) {
      var intervention = await _context.Interventions.FindAsync(dto.InterventionId);
      if (intervention == null) {
         return BadRequest($"Intervention with ID {dto.InterventionId} not found.");
      }

      var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
      if (vehicle == null) {
         return BadRequest($"Vehicle with ID {dto.VehicleId} not found.");
      }

      var assignee = new Assigned {
         Id = Guid.NewGuid(),
         CreatedAt = DateTime.UtcNow,
         InterventionId = dto.InterventionId,
         VehicleId = dto.VehicleId,
         Itinerary = dto.Itinerary,
         Begin = dto.Begin,
         End = dto.End
      };

      _context.Assignees.Add(assignee);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetAssignee), new { id = assignee.Id }, assignee);
   }

   /// <summary>
   /// Met à jour une assignation existante
   /// </summary>
   [HttpPut("{id}")]
   public async Task<IActionResult> UpdateAssignee(Guid id, UpdateAssignedDto dto) {
      var assignee = await _context.Assignees.FindAsync(id);
      if (assignee == null) {
         return NotFound();
      }

      if (dto.InterventionId.HasValue) {
         var intervention = await _context.Interventions.FindAsync(dto.InterventionId.Value);
         if (intervention == null) {
            return BadRequest($"Intervention with ID {dto.InterventionId.Value} not found.");
         }
         assignee.InterventionId = dto.InterventionId.Value;
      }

      if (dto.VehicleId.HasValue) {
         var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId.Value);
         if (vehicle == null) {
            return BadRequest($"Vehicle with ID {dto.VehicleId.Value} not found.");
         }
         assignee.VehicleId = dto.VehicleId.Value;
      }

      if (dto.Itinerary != null)
         assignee.Itinerary = dto.Itinerary;
      if (dto.Begin.HasValue)
         assignee.Begin = dto.Begin.Value;
      if (dto.End.HasValue)
         assignee.End = dto.End.Value;

      assignee.UpdatedAt = DateTime.UtcNow;

      try {
         await _context.SaveChangesAsync();
      } catch (DbUpdateConcurrencyException) {
         if (!await AssigneeExists(id)) {
            return NotFound();
         }
         throw;
      }

      return NoContent();
   }

   [HttpPut("{id}/complete")]
   public async Task<IActionResult> CompleteAssignment(Guid id) {
      var assignee = await _context.Assignees.FindAsync(id);
      if (assignee == null) {
         return NotFound();
      }

      assignee.End = DateTime.UtcNow;
      assignee.UpdatedAt = DateTime.UtcNow;

      var hasNotCompletedAssignments = await _context.Assignees
          .AnyAsync(a => a.InterventionId == assignee.InterventionId && a.End == null && a.Id != id);

      if (!hasNotCompletedAssignments) {
         await _interventionService.CompleteInterventionAsync(assignee.InterventionId);
      }
      try {
         await _context.SaveChangesAsync();
      } catch (DbUpdateConcurrencyException) {
         if (!await AssigneeExists(id)) {
            return NotFound();
         }
         throw;
      }
      return NoContent();
   }

   /// <summary>
   /// Supprime une assignation
   /// </summary>
   [HttpDelete("{id}")]
   public async Task<IActionResult> DeleteAssignee(Guid id) {
      var assignee = await _context.Assignees.FindAsync(id);
      if (assignee == null) {
         return NotFound();
      }

      _context.Assignees.Remove(assignee);
      await _context.SaveChangesAsync();

      return NoContent();
   }

   [HttpGet("incident/{incidentId}")]
   public async Task<ActionResult<IEnumerable<Assigned>>> GetAssigneesByIncident(Guid incidentId) {
      var interventions = await _context.Interventions
          .Where(i => i.IncidentId == incidentId)
          .Select(i => i.Id)
          .ToListAsync();

      var assignees = await _context.Assignees
          .Where(a => interventions.Contains(a.InterventionId))
          .ToListAsync();
      return assignees;
   }

   private async Task<bool> AssigneeExists(Guid id) {
      return await _context.Assignees.AnyAsync(e => e.Id == id);
   }
}
