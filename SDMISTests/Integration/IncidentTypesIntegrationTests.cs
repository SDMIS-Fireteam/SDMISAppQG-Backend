using SDMISAppQG.Models.Entities;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class IncidentTypesIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public IncidentTypesIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CanInsertAndGetAndDeleteIncidentType() {
      // Arrange
      var context = _fixture.Context;

      var incidentTypeId = Guid.NewGuid();
      var incidentType = new IncidentTypeEntity {
         Id = incidentTypeId,
         CreatedAt = DateTime.UtcNow,
         Label = "Test Incident Type",
         Category = "Test Category"
      };

      // Act - Insert
      context.IncidentTypes.Add(incidentType);
      await context.SaveChangesAsync();

      // Assert - Inserted
      var insertedType = await context.IncidentTypes.FindAsync(incidentTypeId);
      Assert.NotNull(insertedType);
      Assert.Equal("Test Incident Type", insertedType.Label);
      Assert.Equal("Test Category", insertedType.Category);

      // Act - Delete
      context.IncidentTypes.Remove(insertedType);
      await context.SaveChangesAsync();

      // Assert - Deleted
      var deletedType = await context.IncidentTypes.FindAsync(incidentTypeId);
      Assert.Null(deletedType);
   }
}
