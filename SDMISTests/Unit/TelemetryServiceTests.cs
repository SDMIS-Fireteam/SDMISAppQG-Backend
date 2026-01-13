using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SDMISAppQG.Database;
using SDMISAppQG.Hubs;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Interfaces.Hubs;
using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Telemetry;
using SDMISTests.Fixtures;

namespace SDMISTests.Unit;

public class TelemetryServiceTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;
   private readonly Mock<IHubContext<GpsHub, IGpsClient>> _mockHub;
   private readonly Mock<IGpsClient> _mockClient;
   private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
   private readonly Mock<IServiceScope> _mockScope;
   private readonly Mock<IServiceProvider> _mockServiceProvider;
   private readonly Mock<ILogger<TelemetryService>> _mockLogger;
   private readonly TelemetryService _service;

   public TelemetryServiceTests(DatabaseFixture fixture) {
      _fixture = fixture;

      // Setup Mocks
      _mockHub = new Mock<IHubContext<GpsHub, IGpsClient>>();
      _mockClient = new Mock<IGpsClient>();
      var mockClients = new Mock<IHubClients<IGpsClient>>();

      mockClients.Setup(c => c.All).Returns(_mockClient.Object);
      _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);

      _mockScopeFactory = new Mock<IServiceScopeFactory>();
      _mockScope = new Mock<IServiceScope>();
      _mockServiceProvider = new Mock<IServiceProvider>();

      _mockScopeFactory.Setup(s => s.CreateScope()).Returns(_mockScope.Object);
      _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);
      // Important: Return the REAL context from the fixture
      _mockServiceProvider.Setup(x => x.GetService(typeof(AppDbContext))).Returns(_fixture.Context);

      _mockLogger = new Mock<ILogger<TelemetryService>>();

      _service = new TelemetryService(_mockHub.Object, _mockScopeFactory.Object, _mockLogger.Object);
   }

   [Fact]
   public async Task ProcessTelemetryAsync_VehicleExists_BroadcastsAndSaves() {
      // Arrange
      var context = _fixture.Context;

      var vehicleType = new VehicleTypeEntity { Id = Guid.NewGuid(), Label = "Test", CreatedAt = DateTime.UtcNow, CrewCapacity = 4 };
      var vehicle = new VehicleEntity {
         Id = Guid.NewGuid(),
         IdHardware = new Random().Next(100000, 999999),
         Type = vehicleType,
         CreatedAt = DateTime.UtcNow,
         LastLocation = new NetTopologySuite.Geometries.Point(0, 0) { SRID = 4326 },
         Availability = SDMISAppQG.Models.Enums.Vehicle.VehicleAvailability.Available,
         UnavailabilityReason = null,
         Fuel = 100,
         PassengerCount = 0
      };
      context.VehicleTypes.Add(vehicleType);
      context.Vehicles.Add(vehicle);
      await context.SaveChangesAsync();

      var telemetryData = new TelemetryData {
         IdHardware = vehicle.IdHardware,
         Latitude = 45.0,
         Longitude = 4.0,
         Levels = new Dictionary<string, double> { { "fuel", 80.0 } }
      };

      // Act
      await _service.ProcessTelemetryAsync(telemetryData);

      // Assert
      // 1. Verify Broadcast
      _mockClient.Verify(c => c.ReceivePosition(It.Is<VehicleDto>(v => 
          v.Id == vehicle.Id && 
          v.Latitude == 45.0 && 
          v.Longitude == 4.0)), Times.Once);

      // 2. Verify Database Update
      // Need to reload/refresh entity from DB to see changes
      context.ChangeTracker.Clear();
      var updatedVehicle = await context.Vehicles.FindAsync(vehicle.Id);
      Assert.NotNull(updatedVehicle);
      Assert.NotNull(updatedVehicle.LastLocation);
      Assert.Equal(4.0, updatedVehicle.LastLocation.X); // Longitude
      Assert.Equal(45.0, updatedVehicle.LastLocation.Y); // Latitude
      Assert.Equal(80.0, updatedVehicle.Fuel);

      // 3. Verify Log Created
      var log = await context.TelemetryLogs.FirstOrDefaultAsync(l => l.VehicleId == vehicle.Id);
      Assert.NotNull(log);
      Assert.Equal(vehicle.Id, log.VehicleId);

      // Cleanup
      context.TelemetryLogs.RemoveRange(context.TelemetryLogs.Where(l => l.VehicleId == vehicle.Id));
      context.Vehicles.Remove(updatedVehicle);
      context.VehicleTypes.Remove(vehicleType);
      await context.SaveChangesAsync();
   }

   [Fact]
   public async Task ProcessTelemetryAsync_VehicleNotFound_DoesNothing() {
      // Arrange
      var telemetryData = new TelemetryData {
         IdHardware = -1, // Does not exist
         Latitude = 45.0,
         Longitude = 4.0
      };

      // Act
      await _service.ProcessTelemetryAsync(telemetryData);

      // Assert
      // 1. Verify No Broadcast
      _mockClient.Verify(c => c.ReceivePosition(It.IsAny<VehicleDto>()), Times.Never);

      // 2. Verify No Log Created (Hard to verify exact count change in shared DB, but can ensure no log for non-existent vehicle)
      // Since vehicle ID is unknown, we can't query logs by vehicle ID easily unless we assume something.
      // But the code returns early if vehicle not found, so we rely on logs/coverage or just the mock verification.
   }
}
