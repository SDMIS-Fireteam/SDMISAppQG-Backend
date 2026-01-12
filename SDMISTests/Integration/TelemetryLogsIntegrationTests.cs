using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SDMISAppQG.Controllers;
using SDMISAppQG.Models;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISTests.Fixtures;
using Xunit;

namespace SDMISTests.Integration;

public class TelemetryLogsIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public TelemetryLogsIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanInsertAndGetTelemetryLog()
    {
        // Arrange
        var context = _fixture.Context;

        // Use seeded vehicle: 77777777-7777-7777-7777-777777777777
        var vehicleId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var vehicle = await context.Vehicles.FindAsync(vehicleId);
        Assert.NotNull(vehicle);

        var logId = Guid.NewGuid();
        var log = new TelemetryLogsEntity
        {
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

    [Fact]
    public async Task CanGetTelemetryLogsByVehicle()
    {
        // Arrange
        var context = _fixture.Context;
        var controller = new TelemetryLogsController(context);

        // Seed vehicle
        var vehicleType = new VehicleTypeEntity { Id = Guid.NewGuid(), Label = "TestLog", CreatedAt = DateTime.UtcNow, CrewCapacity = 2 };
        context.VehicleTypes.Add(vehicleType);
        var vehicle = new VehicleEntity
        {
            Id = Guid.NewGuid(),
            IdHardware = new Random().Next(1000000, 9999999),
            Type = vehicleType,
            CreatedAt = DateTime.UtcNow,
            LastLocation = new Point(0, 0) { SRID = 4326 },
            Availability = SDMISAppQG.Models.Enums.Vehicle.VehicleAvailability.Available,
            UnavailabilityReason = null,
            Fuel = 100,
            PassengerCount = 0
        };
        context.Vehicles.Add(vehicle);

        // Seed logs
        var log1 = new TelemetryLogsEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            CreatedAt = DateTime.UtcNow,
            Position = new Point(1, 1) { SRID = 4326 },
            Timestamp = DateTime.UtcNow.AddMinutes(-10),
            SensorsValues = new List<SensorValue>()
        };
        var log2 = new TelemetryLogsEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicle.Id,
            CreatedAt = DateTime.UtcNow,
            Position = new Point(2, 2) { SRID = 4326 },
            Timestamp = DateTime.UtcNow, // Newer
            SensorsValues = new List<SensorValue>()
        };
        context.TelemetryLogs.AddRange(log1, log2);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.GetTelemetryLogsByVehicle(vehicle.Id);

        // Assert
        var okResult = Assert.IsAssignableFrom<IEnumerable<TelemetryLogsEntity>>(result.Value);
        var logs = okResult.ToList();
        Assert.Equal(2, logs.Count);
        Assert.Equal(log2.Id, logs[0].Id); // Ordered by Timestamp Descending
        Assert.Equal(log1.Id, logs[1].Id);

        // Cleanup
        context.TelemetryLogs.RemoveRange(logs);
        context.Vehicles.Remove(vehicle);
        context.VehicleTypes.Remove(vehicleType);
        await context.SaveChangesAsync();
    }
}