using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums.Incidents;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class IncidentsIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public IncidentsIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CanInsertAndGetAndDeleteIncident() {
      // Arrange
      var context = _fixture.Context;

      // Use an existing IncidentType ID (from seeding) or create a new one. 
      // Using one from seeding to be safe: 11111111-1111-1111-1111-111111111111
      var typeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
      var incidentType = await context.IncidentTypes.FindAsync(typeId);
      Assert.NotNull(incidentType);

      var incidentId = Guid.NewGuid();
      var incident = new IncidentEntity {
         Id = incidentId,
         Type = incidentType,
         CreatedAt = DateTime.UtcNow,
         Location = new Point(4.8357, 45.7640) { SRID = 4326 }, // Lyon
         Severity = IncidentSeverity.Low,
         Priority = 1.0f,
         Status = IncidentStatus.Declared,
         Source = IncidentSource.Internal,
         Description = "Integration Test Incident"
      };

      // Act - Insert
      context.Incidents.Add(incident);
      await context.SaveChangesAsync();

      // Assert - Inserted
      var insertedIncident = await context.Incidents.FindAsync(incidentId);
      Assert.NotNull(insertedIncident);
      Assert.Equal("Integration Test Incident", insertedIncident.Description);

      // Act - Delete
      context.Incidents.Remove(insertedIncident);
      await context.SaveChangesAsync();

      // Assert - Deleted
      var deletedIncident = await context.Incidents.FindAsync(incidentId);
      Assert.Null(deletedIncident);
   }

   [Fact]
   public async Task CanUpdateIncident() {
      // Arrange
      var context = _fixture.Context;
      var typeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
      var incidentType = await context.IncidentTypes.FindAsync(typeId);
      Assert.NotNull(incidentType);

      var incidentId = Guid.NewGuid();
      var incident = new IncidentEntity {
         Id = incidentId,
         Type = incidentType,
         CreatedAt = DateTime.UtcNow,
         Location = new Point(4.8357, 45.7640) { SRID = 4326 },
         Severity = IncidentSeverity.Medium,
         Priority = 2.0f,
         Status = IncidentStatus.Declared,
         Source = IncidentSource.Internal,
         Description = "Original Description"
      };

      context.Incidents.Add(incident);
      await context.SaveChangesAsync();

      // Act - Update
      var incidentToUpdate = await context.Incidents.FindAsync(incidentId);
      Assert.NotNull(incidentToUpdate);
      incidentToUpdate.Description = "Updated Description";
      incidentToUpdate.Status = IncidentStatus.Ongoing;
      await context.SaveChangesAsync();

      // Assert - Updated
      var updatedIncident = await context.Incidents.FindAsync(incidentId);
      Assert.NotNull(updatedIncident);
      Assert.Equal("Updated Description", updatedIncident.Description);
      Assert.Equal(IncidentStatus.Ongoing, updatedIncident.Status);

      // Cleanup
      context.Incidents.Remove(updatedIncident);
      await context.SaveChangesAsync();
   }
}
