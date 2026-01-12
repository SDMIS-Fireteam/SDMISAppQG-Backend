using SDMISAppQG.Models.Entities;
using SDMISTests.Fixtures;
using Xunit;

namespace SDMISTests.Integration;

public class VehicleTypesIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public VehicleTypesIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanInsertAndGetAndDeleteVehicleType()
    {
        // Arrange
        var context = _fixture.Context;
        
        var vehicleTypeId = Guid.NewGuid();
        var vehicleType = new VehicleTypeEntity
        {
            Id = vehicleTypeId,
            CreatedAt = DateTime.UtcNow,
            Label = "Test Vehicle Type",
            CrewCapacity = 5,
            Consumables = new List<string> { "TestConsumable" }
        };

        // Act - Insert
        context.VehicleTypes.Add(vehicleType);
        await context.SaveChangesAsync();

        // Assert - Inserted
        var insertedType = await context.VehicleTypes.FindAsync(vehicleTypeId);
        Assert.NotNull(insertedType);
        Assert.Equal("Test Vehicle Type", insertedType.Label);
        Assert.Equal(5, insertedType.CrewCapacity);
        Assert.NotNull(insertedType.Consumables);
        Assert.Contains("TestConsumable", insertedType.Consumables);

        // Act - Delete
        context.VehicleTypes.Remove(insertedType);
        await context.SaveChangesAsync();

        // Assert - Deleted
        var deletedType = await context.VehicleTypes.FindAsync(vehicleTypeId);
        Assert.Null(deletedType);
    }
}
