using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SDMISAppQG.Controllers;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISAppQG.Models.Enums.Incidents;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class InterventionCompletionIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public InterventionCompletionIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CompleteIntervention_UpdatesIncidentStatusAndClosesAssignments() {
      // Arrange
      var context = _fixture.Context;
      var interventionService = new InterventionService(context);
      var controller = new InterventionsController(context, interventionService);

      // 1. Create Incident
      var incidentType = new IncidentTypeEntity { Id = Guid.NewGuid(), Label = "Fire", Category = "Urgent", CreatedAt = DateTime.UtcNow };
      context.IncidentTypes.Add(incidentType);
      var incidentId = Guid.NewGuid();
      var incident = new IncidentEntity {
         Id = incidentId,
         Type = incidentType,
         CreatedAt = DateTime.UtcNow,
         Location = new Point(4.0, 45.0) { SRID = 4326 },
         Severity = IncidentSeverity.High,
         Priority = 10,
         Status = IncidentStatus.Ongoing, // Initially Ongoing
         Source = IncidentSource.External
      };
      context.Incidents.Add(incident);

      // 2. Create Intervention
      var interventionId = Guid.NewGuid();
      var intervention = new InterventionEntity {
         Id = interventionId,
         IncidentId = incidentId,
         CreatedAt = DateTime.UtcNow,
         Begin = DateTime.UtcNow,
         Status = InterventionStatus.Ongoing
      };
      context.Interventions.Add(intervention);

      // 3. Create Active Assignment
      var vehicleId = Guid.Parse("77777777-7777-7777-7777-777777777777"); // Seeded vehicle
      var assignment = new Assigned {
         Id = Guid.NewGuid(),
         InterventionId = interventionId,
         VehicleId = vehicleId,
         CreatedAt = DateTime.UtcNow,
         Begin = DateTime.UtcNow,
         End = null // Active
      };
      context.Assignees.Add(assignment);

      await context.SaveChangesAsync();

      // Act
      var result = await controller.CompleteIntervention(interventionId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      // Use reflection to check properties of anonymous type result if needed, or just trust the DB state assertions

      // 1. Verify Intervention Status
      var updatedIntervention = await context.Interventions.FindAsync(interventionId);
      Assert.Equal(InterventionStatus.Completed, updatedIntervention!.Status);
      Assert.NotNull(updatedIntervention.End);

      // 2. Verify Assignment Closed
      var updatedAssignment = await context.Assignees.FindAsync(assignment.Id);
      Assert.NotNull(updatedAssignment!.End);

      // 3. Verify Incident Status Updated
      var updatedIncident = await context.Incidents.FindAsync(incidentId);
      Assert.Equal(IncidentStatus.Completed, updatedIncident!.Status);

      // Cleanup
      context.Assignees.Remove(updatedAssignment);
      context.Interventions.Remove(updatedIntervention);
      context.Incidents.Remove(updatedIncident);
      context.IncidentTypes.Remove(incidentType);
      await context.SaveChangesAsync();
   }
}
