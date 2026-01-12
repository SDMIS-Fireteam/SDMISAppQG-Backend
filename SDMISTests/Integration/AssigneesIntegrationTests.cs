using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISTests.Fixtures;
using Xunit;

namespace SDMISTests.Integration;

public class AssigneesIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public AssigneesIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanAssignToIntervention()
    {
        // Arrange
        var context = _fixture.Context;

        // 1. Get seeded entities
        var vehicleId = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var incidentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        
        var interventionId = Guid.NewGuid();
        var intervention = new InterventionEntity
        {
            Id = interventionId,
            IncidentId = incidentId,
            CreatedAt = DateTime.UtcNow,
            Begin = DateTime.UtcNow,
            Status = InterventionStatus.Pending
        };
        context.Interventions.Add(intervention);

        var assigneeId = Guid.NewGuid();
        var assignee = new Assigned
        {
            Id = assigneeId,
            InterventionId = interventionId,
            VehicleId = vehicleId,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Assignees.Add(assignee);
        await context.SaveChangesAsync();

        // Assert
        var inserted = await context.Assignees.FindAsync(assigneeId);
        Assert.NotNull(inserted);
        Assert.Equal(interventionId, inserted.InterventionId);
        Assert.Equal(vehicleId, inserted.VehicleId);

        // Cleanup
        context.Assignees.Remove(inserted);
        context.Interventions.Remove(intervention);
        await context.SaveChangesAsync();
    }
}