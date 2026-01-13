using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SDMISAppQG.Controllers;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISAppQG.Models.Enums.Incidents;
using SDMISAppQG.Models.Enums.Vehicle;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class AssigneeCompletionIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public AssigneeCompletionIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CompleteLastAssignment_CompletesInterventionAndIncident() {
      // Arrange
      var context = _fixture.Context;

      // Setup services (simulating DI)
      var interventionService = new InterventionService(context);
      var assigneesController = new AssigneesController(context, interventionService);

      // 1. Create Incident
      var incidentType = new IncidentTypeEntity { Id = Guid.NewGuid(), Label = "Accident", Category = "Road", CreatedAt = DateTime.UtcNow };
      context.IncidentTypes.Add(incidentType);
      var incidentId = Guid.NewGuid();
      var incident = new IncidentEntity {
         Id = incidentId,
         Type = incidentType,
         CreatedAt = DateTime.UtcNow,
         Location = new Point(4.0, 45.0) { SRID = 4326 },
         Severity = IncidentSeverity.Medium,
         Priority = 5,
         Status = IncidentStatus.Ongoing,
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

      // 3. Create 2 Assignments
      var vehicleId1 = Guid.NewGuid(); // Simplified vehicle creation for brevity (assuming FK constraint doesn't strict check on InMemory/Tests if seeded? Wait, I'm using REAL DB. I need valid vehicles.)
      var vehicleId2 = Guid.NewGuid();

      // Create Vehicle Types/Vehicles properly
      var vType = new VehicleTypeEntity { Id = Guid.NewGuid(), Label = "Car", CreatedAt = DateTime.UtcNow, CrewCapacity = 2 };
      context.VehicleTypes.Add(vType);

      var v1 = new VehicleEntity { Id = vehicleId1, IdHardware = 101, Type = vType, CreatedAt = DateTime.UtcNow, LastLocation = new Point(0, 0) { SRID = 4326 }, Availability = VehicleAvailability.Available, UnavailabilityReason = null, Fuel = 100, PassengerCount = 0 };
      var v2 = new VehicleEntity { Id = vehicleId2, IdHardware = 102, Type = vType, CreatedAt = DateTime.UtcNow, LastLocation = new Point(0, 0) { SRID = 4326 }, Availability = VehicleAvailability.Available, UnavailabilityReason = null, Fuel = 100, PassengerCount = 0 };
      context.Vehicles.AddRange(v1, v2);

      var assignment1 = new Assigned { Id = Guid.NewGuid(), InterventionId = interventionId, VehicleId = vehicleId1, CreatedAt = DateTime.UtcNow, Begin = DateTime.UtcNow };
      var assignment2 = new Assigned { Id = Guid.NewGuid(), InterventionId = interventionId, VehicleId = vehicleId2, CreatedAt = DateTime.UtcNow, Begin = DateTime.UtcNow };
      context.Assignees.AddRange(assignment1, assignment2);

      await context.SaveChangesAsync();

      // Act - Complete Assignment 1 (Intervention should stay Ongoing)
      await assigneesController.CompleteAssignment(assignment1.Id);

      var checkIntervention = await context.Interventions.FindAsync(interventionId);
      Assert.Equal(InterventionStatus.Ongoing, checkIntervention!.Status);

      // Act - Complete Assignment 2 (Last one -> Intervention should become Completed, Incident Completed)
      await assigneesController.CompleteAssignment(assignment2.Id);

      // Assert
      var finalIntervention = await context.Interventions.FindAsync(interventionId);
      Assert.Equal(InterventionStatus.Completed, finalIntervention!.Status);
      Assert.NotNull(finalIntervention.End);

      var finalIncident = await context.Incidents.FindAsync(incidentId);
      Assert.Equal(IncidentStatus.Completed, finalIncident!.Status);

      // Cleanup
      context.Assignees.RemoveRange(assignment1, assignment2);
      context.Interventions.Remove(finalIntervention);
      context.Incidents.Remove(finalIncident);
      context.Vehicles.RemoveRange(v1, v2);
      context.VehicleTypes.Remove(vType);
      context.IncidentTypes.Remove(incidentType);
      await context.SaveChangesAsync();
   }
}
