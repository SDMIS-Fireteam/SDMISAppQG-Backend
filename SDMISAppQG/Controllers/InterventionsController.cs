using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InterventionsController : ControllerBase {
   private readonly AppDbContext _context;
   private readonly InterventionService _interventionService;

   public InterventionsController(AppDbContext context, InterventionService interventionService) {
      _context = context;
      _interventionService = interventionService;
   }

   /// <summary>
   /// Récupère toutes les interventions
   /// </summary>
   [HttpGet]
   public async Task<ActionResult<IEnumerable<InterventionEntity>>> GetInterventions() {
      return await _context.Interventions.ToListAsync();
   }

   /// <summary>
   /// Récupère une intervention par son ID
   /// </summary>
   [HttpGet("{id}")]
   public async Task<ActionResult<InterventionEntity>> GetIntervention(Guid id) {
      var intervention = await _context.Interventions.FindAsync(id);

      if (intervention == null) {
         return NotFound();
      }

      return intervention;
   }

   /// <summary>
   /// Récupère toutes les interventions pour un incident spécifique
   /// </summary>
   [HttpGet("incident/{incidentId}")]
   public async Task<ActionResult<IEnumerable<InterventionEntity>>> GetInterventionsByIncident(Guid incidentId) {
      return await _context.Interventions
         .Where(i => i.IncidentId == incidentId)
         .ToListAsync();
   }

   /// <summary>
   /// Crée une nouvelle intervention
   /// </summary>
   [HttpPost]
   public async Task<ActionResult<InterventionEntity>> CreateIntervention(CreateInterventionDto dto) {
      var intervention = new InterventionEntity {
         Id = Guid.NewGuid(),
         CreatedAt = DateTime.UtcNow,
         IncidentId = dto.IncidentId,
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
   public async Task<IActionResult> UpdateIntervention(Guid id, UpdateInterventionDto dto) {
      var intervention = await _context.Interventions.FindAsync(id);
      if (intervention == null) {
         return NotFound();
      }

      if (dto.IncidentId.HasValue)
         intervention.IncidentId = dto.IncidentId.Value;
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

      try {
         await _context.SaveChangesAsync();
      } catch (DbUpdateConcurrencyException) {
         if (!await InterventionExists(id)) {
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
   public async Task<IActionResult> DeleteIntervention(Guid id) {
      var intervention = await _context.Interventions.FindAsync(id);
      if (intervention == null) {
         return NotFound();
      }

      _context.Interventions.Remove(intervention);
      await _context.SaveChangesAsync();

      return NoContent();
   }

   [HttpGet("vehicle/{vehicleId}")]
   public async Task<ActionResult<IEnumerable<InterventionEntity>>> GetInterventionsByVehicle(Guid vehicleId) {
      var interventions = await _context.Interventions
          .Where(i => _context.Assignees
              .Any(a => a.InterventionId == i.Id && a.VehicleId == vehicleId))
          .ToListAsync();
      return interventions;
   }

   /// <summary>
   /// Assigne un véhicule à un incident en créant une intervention
   /// </summary>
   [HttpPost("{incidentId}/createIntervention/{vehicleId}")]
   public async Task<IActionResult> AssignVehicleToIncident(Guid incidentId, Guid vehicleId) {
      var incident = await _context.Incidents.FindAsync(incidentId);
      if (incident == null) {
         return NotFound($"Incident with ID {incidentId} not found.");
      }

      var vehicle = await _context.Vehicles.FindAsync(vehicleId);
      if (vehicle == null) {
         return NotFound($"Vehicle with ID {vehicleId} not found.");
      }

      // Vérifier si le véhicule est déjà assigné à n'importe quelle autre intervention en cours
      var isVehicleBusy = await _context.Assignees
          .AnyAsync(a => a.VehicleId == vehicleId && a.End == null);

      if (isVehicleBusy) {
         return BadRequest($"Vehicle with ID {vehicleId} is already assigned to another ongoing intervention.");
      }

      var intervention = await _context.Interventions
          .FirstOrDefaultAsync(i => i.IncidentId == incidentId && i.Status != InterventionStatus.Completed);

      if (intervention == null) {
         intervention = new InterventionEntity {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IncidentId = incidentId,
            Begin = DateTime.UtcNow,
            Status = InterventionStatus.Ongoing
         };

         _context.Interventions.Add(intervention);
      } else {
         // Vérifier si le véhicule est déjà assigné à cette intervention
         var alreadyAssigned = await _context.Assignees
             .AnyAsync(a => a.InterventionId == intervention.Id && a.VehicleId == vehicleId && a.End == null);

         if (alreadyAssigned) {
            return BadRequest($"Vehicle with ID {vehicleId} is already assigned to an ongoing intervention for this incident.");
         }
      }

      // Créer l'itinéraire au format MapBox avec waypoints
      var waypoints = new[]
      {
            new { lat = vehicle.LastLocation?.Y ?? 0, lon = vehicle.LastLocation?.X ?? 0 }, // Position du véhicule
            new { lat = incident.Location?.Y ?? 0, lon = incident.Location?.X ?? 0 }        // Position de l'incident
        };

      var itinerary = new {
         waypoints = waypoints,
      };

      var assigment = new Assigned {
         Id = Guid.NewGuid(),
         CreatedAt = DateTime.UtcNow,
         Itinerary = System.Text.Json.JsonSerializer.Serialize(itinerary),
         InterventionId = intervention.Id,
         VehicleId = vehicleId,
         Begin = DateTime.UtcNow
      };

      _context.Assignees.Add(assigment);

      await _context.SaveChangesAsync();
      return NoContent();
   }

   /// <summary>
   /// Termine une intervention en mettant à jour son statut et sa date de fin
   /// </summary>
   [HttpPost("{id}/complete")]
   public async Task<IActionResult> CompleteIntervention(Guid id) {
      var result = await _interventionService.CompleteInterventionAsync(id);

      if (!result) {
         // Either not found or issue (Service returns bool, simple check)
         // If strictly not found, service returns false.
         // If checking specifically for NotFound vs Success, service could return nullable/enum.
         // For now, assuming false means not found or failed.
         // Check if exists
         if (!await InterventionExists(id)) return NotFound();

         // If exists but returned false, maybe already completed? Service returns true if already completed.
         // So false really means not found in my implementation.
         return NotFound($"Intervention with ID {id} not found.");
      }

      // Fetch updated data for response
      var intervention = await _context.Interventions.FindAsync(id);

      return Ok(new {
         message = "Intervention completed successfully",
         interventionId = id,
         completedAt = intervention?.End,
         // assignmentsCompleted not strictly returned by service, can omit or fetch
      });
   }

   private async Task<bool> InterventionExists(Guid id) {
      return await _context.Interventions.AnyAsync(e => e.Id == id);
   }
}
