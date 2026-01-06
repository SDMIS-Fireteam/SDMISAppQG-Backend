using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InterventionsController : ControllerBase {
   private readonly AppDbContext _context;

   public InterventionsController(AppDbContext context) {
      _context = context;
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
   public async Task<ActionResult<InterventionEntity>> CreateIntervention(InterventionEntity intervention) {
      intervention.Id = Guid.NewGuid();
      _context.Interventions.Add(intervention);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetIntervention), new { id = intervention.Id }, intervention);
   }

   /// <summary>
   /// Met à jour une intervention existante
   /// </summary>
   [HttpPut("{id}")]
   public async Task<IActionResult> UpdateIntervention(Guid id, InterventionEntity intervention) {
      if (id != intervention.Id) {
         return BadRequest();
      }

      _context.Entry(intervention).State = EntityState.Modified;

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

   private async Task<bool> InterventionExists(Guid id) {
      return await _context.Interventions.AnyAsync(e => e.Id == id);
   }
}
