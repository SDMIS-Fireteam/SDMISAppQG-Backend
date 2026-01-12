using NetTopologySuite.Geometries;
using SDMISAppQG.Models;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class TelemetryLogsIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public TelemetryLogsIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CanInsertAndGetTelemetryLog() {
      // Arrange
      var context = _fixture.Context;

      // Use seeded vehicle: 77777777-7777-7777-7777-777777777777
      var vehicleId = Guid.Parse("77777777-7777-7777-7777-777777777777");
      var vehicle = await context.Vehicles.FindAsync(vehicleId);
      Assert.NotNull(vehicle);

      var logId = Guid.NewGuid();
      var log = new TelemetryLogsEntity {
         Id = logId,
         VehicleId = vehicleId,
         CreatedAt = DateTime.UtcNow,
         Position = new Point(4.8357, 45.7640) { SRID = 4326 },
         Timestamp = DateTime.UtcNow,
         SensorsValues = new List<SensorValue>
          {
                new SensorValue { Code = "SPEED", Valeur = 50.0, Unite = UnitOfMeasurement.Unknown }
            }
      };

      // Act - Insert
      context.TelemetryLogs.Add(log);
      await context.SaveChangesAsync();

      // Assert - Inserted
      var inserted = await context.TelemetryLogs.FindAsync(logId);
      Assert.NotNull(inserted);
      Assert.Equal(vehicleId, inserted.VehicleId);
      Assert.Single(inserted.SensorsValues);
      Assert.Equal("SPEED", inserted.SensorsValues[0].Code);

      // Cleanup
      context.TelemetryLogs.Remove(inserted);
      await context.SaveChangesAsync();
   }
}