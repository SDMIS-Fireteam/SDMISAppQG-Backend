using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class InterventionsIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public InterventionsIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
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