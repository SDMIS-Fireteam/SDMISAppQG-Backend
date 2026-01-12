using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Enums;
using Xunit;

namespace SDMISTests.Unit;

public class DtoTests
{
    [Fact]
    public void CreateUserDto_ShouldStoreDataCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateUserDto
        {
            KeyCloakId = id,
            Email = "test@example.com",
            Username = "testuser",
            Firstname = "Test",
            Lastname = "User",
            Role = UserRole.Firefighter
        };

        // Assert
        Assert.Equal(id, dto.KeyCloakId);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("testuser", dto.Username);
        Assert.Equal(UserRole.Firefighter, dto.Role);
    }

    [Fact]
    public void CreateIncidentDto_ShouldStoreDataCorrectly()
    {
        // Arrange
        var typeId = Guid.NewGuid();
        var location = new NetTopologySuite.Geometries.Point(4.0, 45.0) { SRID = 4326 };
        var dto = new CreateIncidentDto
        {
            TypeId = typeId,
            Location = location,
            Severity = SDMISAppQG.Models.Enums.Incidents.IncidentSeverity.High,
            Priority = 10.0f,
            Status = SDMISAppQG.Models.Enums.Incidents.IncidentStatus.Declared,
            Source = SDMISAppQG.Models.Enums.Incidents.IncidentSource.External,
            Description = "Test Incident"
        };

        // Assert
        Assert.Equal(typeId, dto.TypeId);
        Assert.Equal(location, dto.Location);
        Assert.Equal("Test Incident", dto.Description);
        Assert.Equal(SDMISAppQG.Models.Enums.Incidents.IncidentStatus.Declared, dto.Status);
    }
}
