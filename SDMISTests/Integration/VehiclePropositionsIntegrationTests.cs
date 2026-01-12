using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class VehiclePropositionsIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public VehiclePropositionsIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CanInsertAndGetAndDeleteVehicleProposition() {
      // Arrange
      var context = _fixture.Context;

      // 1. Need an Incident
      var incidentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // From seeding
      var incident = await context.Incidents.FindAsync(incidentId);
      Assert.NotNull(incident);

      // 2. Need an Intervention
      var interventionId = Guid.NewGuid();
      var intervention = new InterventionEntity {
         Id = interventionId,
         IncidentId = incidentId,
         CreatedAt = DateTime.UtcNow,
         Begin = DateTime.UtcNow,
         Status = InterventionStatus.Pending
      };
      context.Interventions.Add(intervention);

      // 3. Need a Vehicle
      var vehicleId = Guid.Parse("77777777-7777-7777-7777-777777777777"); // From seeding
      var vehicle = await context.Vehicles.FindAsync(vehicleId);
      Assert.NotNull(vehicle);

      var propositionId = Guid.NewGuid();
      var proposition = new VehiclePropositionEntity {
         Id = propositionId,
         CreatedAt = DateTime.UtcNow,
         InterventionId = interventionId,
         VehicleId = vehicleId,
         Score = 85.5f
      };

      // Act - Insert
      context.VehiclePropositions.Add(proposition);
      await context.SaveChangesAsync();

      // Assert - Inserted
      var inserted = await context.VehiclePropositions.FindAsync(propositionId);
      Assert.NotNull(inserted);
      Assert.Equal(85.5f, inserted.Score);
      Assert.Equal(interventionId, inserted.InterventionId);
      Assert.Equal(vehicleId, inserted.VehicleId);

      // Act - Delete
      context.VehiclePropositions.Remove(inserted);
      await context.SaveChangesAsync();

      // Assert - Deleted
      var deleted = await context.VehiclePropositions.FindAsync(propositionId);
      Assert.Null(deleted);

      // Cleanup
      context.Interventions.Remove(intervention);
      await context.SaveChangesAsync();
   }
}
