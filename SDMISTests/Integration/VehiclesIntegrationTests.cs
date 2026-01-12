using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums.Vehicle;
using SDMISTests.Fixtures;
using Xunit;

namespace SDMISTests.Integration;

public class VehiclesIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public VehiclesIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanInsertAndGetAndDeleteVehicle()
    {
        // Arrange
        var context = _fixture.Context;
        
        // Use an existing VehicleType ID (from seeding): 44444444-4444-4444-4444-444444444444
        var typeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var vehicleType = await context.VehicleTypes.FindAsync(typeId);
        Assert.NotNull(vehicleType);

        var vehicleId = Guid.NewGuid();
        var vehicle = new VehicleEntity
        {
            Id = vehicleId,
            IdHardware = 999, // Test Hardware ID
            Type = vehicleType,
            CreatedAt = DateTime.UtcNow,
            LastLocation = new Point(4.8357, 45.7640) { SRID = 4326 },
            Availability = VehicleAvailability.Available,
            UnavailabilityReason = null,
            Fuel = 100.0f,
            PassengerCount = 0
        };

        // Act - Insert
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Assert - Inserted
        var insertedVehicle = await context.Vehicles.FindAsync(vehicleId);
        Assert.NotNull(insertedVehicle);
        Assert.Equal(999, insertedVehicle.IdHardware);

        // Act - Delete
        context.Vehicles.Remove(insertedVehicle);
        await context.SaveChangesAsync();

        // Assert - Deleted
        var deletedVehicle = await context.Vehicles.FindAsync(vehicleId);
        Assert.Null(deletedVehicle);
    }
}
