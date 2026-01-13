using Microsoft.AspNetCore.Mvc;
using SDMISAppQG.Controllers;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISAppQG.Models.Enums.Vehicle;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class InterventionsIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public InterventionsIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   private InterventionsController GetController() {
      var context = _fixture.Context;
      var interventionService = new InterventionService(context);
      return new InterventionsController(context, interventionService);
   }

   [Fact]
   public async Task AssignVehicleToIncident_PreventsDoubleAssignment() {
      // Arrange
      var context = _fixture.Context;
      var controller = GetController();

      var incidentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // From seeding

      // Create a temporary vehicle to be sure it's not already assigned
      var typeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
      var vehicleType = await context.VehicleTypes.FindAsync(typeId);
      var vehicleId = Guid.NewGuid();
      var vehicle = new VehicleEntity {
         Id = vehicleId,
         IdHardware = 9999,
         Type = vehicleType!,
         CreatedAt = DateTime.UtcNow,
         Availability = VehicleAvailability.Available,
         UnavailabilityReason = null,
         Fuel = 100
      };
      context.Vehicles.Add(vehicle);
      await context.SaveChangesAsync();

      try {
         // Act - First Assignment
         var result1 = await controller.AssignVehicleToIncident(incidentId, vehicleId);
         Assert.IsType<NoContentResult>(result1);

         // Act - Second Assignment (same vehicle, same or different incident)
         var result2 = await controller.AssignVehicleToIncident(incidentId, vehicleId);

         // Assert
         var badRequest = Assert.IsType<BadRequestObjectResult>(result2);
         Assert.Contains("already assigned", badRequest.Value?.ToString());
      } finally {
         // Cleanup
         var assignments = context.Assignees.Where(a => a.VehicleId == vehicleId);
         context.Assignees.RemoveRange(assignments);
         context.Vehicles.Remove(vehicle);
         await context.SaveChangesAsync();
      }
   }

   [Fact]
   public async Task CanInsertAndGetAndDeleteIntervention() {
      // Arrange
      var context = _fixture.Context;

      // 1. Need an Incident
      var incidentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // From seeding
      var incident = await context.Incidents.FindAsync(incidentId);
      Assert.NotNull(incident);

      var interventionId = Guid.NewGuid();
      var intervention = new InterventionEntity {
         Id = interventionId,
         IncidentId = incidentId,
         CreatedAt = DateTime.UtcNow,
         Begin = DateTime.UtcNow,
         Status = InterventionStatus.Ongoing
      };

      // Act - Insert
      context.Interventions.Add(intervention);
      await context.SaveChangesAsync();

      // Assert - Inserted
      var inserted = await context.Interventions.FindAsync(interventionId);
      Assert.NotNull(inserted);
      Assert.Equal(InterventionStatus.Ongoing, inserted.Status);

      // Act - Delete
      context.Interventions.Remove(inserted);
      await context.SaveChangesAsync();

      // Assert - Deleted
      var deleted = await context.Interventions.FindAsync(interventionId);
      Assert.Null(deleted);
   }
}