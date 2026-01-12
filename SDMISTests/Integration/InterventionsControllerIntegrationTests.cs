using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SDMISAppQG.Controllers;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums.Incidents;
using SDMISAppQG.Models.Enums.Vehicle;
using SDMISTests.Fixtures;
using Xunit;

namespace SDMISTests.Integration;

public class InterventionsControllerIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public InterventionsControllerIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AssignVehicleToIncident_CreatesInterventionAndAssignment()
    {
        // Arrange
        var context = _fixture.Context;
        var controller = new InterventionsController(context);

        // 1. Create IncidentType
        var incidentType = new IncidentTypeEntity { Id = Guid.NewGuid(), Label = "Fire", Category = "Urgent", CreatedAt = DateTime.UtcNow };
        context.IncidentTypes.Add(incidentType);

        // 2. Create Incident
        var incidentId = Guid.NewGuid();
        var incident = new IncidentEntity
        {
            Id = incidentId,
            Type = incidentType,
            CreatedAt = DateTime.UtcNow,
            Location = new Point(4.0, 45.0) { SRID = 4326 },
            Severity = IncidentSeverity.High,
            Priority = 10,
            Status = IncidentStatus.Declared,
            Source = IncidentSource.External
        };
        context.Incidents.Add(incident);

        // 3. Create VehicleType
        var vehicleType = new VehicleTypeEntity { Id = Guid.NewGuid(), Label = "Truck", CreatedAt = DateTime.UtcNow, CrewCapacity = 4 };
        context.VehicleTypes.Add(vehicleType);

        // 4. Create Vehicle
        var vehicleId = Guid.NewGuid();
        var vehicle = new VehicleEntity
        {
            Id = vehicleId,
            IdHardware = new Random().Next(1000, 9999),
            Type = vehicleType,
            CreatedAt = DateTime.UtcNow,
            LastLocation = new Point(4.1, 45.1) { SRID = 4326 },
            Availability = VehicleAvailability.Available,
            UnavailabilityReason = null,
            Fuel = 100,
            PassengerCount = 0
        };
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.AssignVehicleToIncident(incidentId, vehicleId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify Intervention created
        var intervention = await context.Interventions.FirstOrDefaultAsync(i => i.IncidentId == incidentId);
        Assert.NotNull(intervention);
        Assert.Equal(SDMISAppQG.Models.Enums.InterventionStatus.Ongoing, intervention.Status);

        // Verify Assignment created
        var assignment = await context.Assignees.FirstOrDefaultAsync(a => a.InterventionId == intervention.Id && a.VehicleId == vehicleId);
        Assert.NotNull(assignment);
        Assert.NotNull(assignment.Itinerary);

        // Cleanup
        context.Assignees.Remove(assignment);
        context.Interventions.Remove(intervention);
        context.Vehicles.Remove(vehicle);
        context.Incidents.Remove(incident);
        context.VehicleTypes.Remove(vehicleType);
        context.IncidentTypes.Remove(incidentType);
        await context.SaveChangesAsync();
    }
}
