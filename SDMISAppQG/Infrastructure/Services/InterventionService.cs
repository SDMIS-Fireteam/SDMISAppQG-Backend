using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Infrastructure.Services;

public class InterventionService {
   private readonly AppDbContext _context;

   public InterventionService(AppDbContext context) {
      _context = context;
   }

   public async Task<bool> CompleteInterventionAsync(Guid interventionId) {
      var intervention = await _context.Interventions.FindAsync(interventionId);
      if (intervention == null) {
         return false;
      }

      if (intervention.Status == InterventionStatus.Completed) {
         return true; // Already completed
      }

      // Update Intervention
      intervention.Status = InterventionStatus.Completed;
      intervention.End = DateTime.UtcNow;
      intervention.UpdatedAt = DateTime.UtcNow;

      // Close all active assignments
      var assignments = await _context.Assignees
          .Where(a => a.InterventionId == interventionId && a.End == null)
          .ToListAsync();

      foreach (var assignment in assignments) {
         assignment.End = DateTime.UtcNow;
         assignment.UpdatedAt = DateTime.UtcNow;
      }

      // Update associated Incident
      var incident = await _context.Incidents.FindAsync(intervention.IncidentId);
      if (incident != null) {
         incident.Status = SDMISAppQG.Models.Enums.Incidents.IncidentStatus.Completed;
         incident.UpdatedAt = DateTime.UtcNow;
      }

      await _context.SaveChangesAsync();
      return true;
   }
}
