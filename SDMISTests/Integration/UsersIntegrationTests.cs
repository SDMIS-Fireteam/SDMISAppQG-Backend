using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SDMISAppQG.Controllers;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums.Vehicle;
using SDMISTests.Fixtures;

namespace SDMISTests.Integration;

public class UsersIntegrationTests : IClassFixture<DatabaseFixture> {
   private readonly DatabaseFixture _fixture;

   public UsersIntegrationTests(DatabaseFixture fixture) {
      _fixture = fixture;
   }

   [Fact]
   public async Task CanGetUserCurrentVehicle() {
      // Arrange
      var context = _fixture.Context;
      var controller = new UsersController(context);

      // 1. Create User
      var userId = Guid.NewGuid();
      var user = new UserEntity {
         Id = userId,
         CreatedAt = DateTime.UtcNow,
         KeyCloakId = Guid.NewGuid(),
         Email = $"test.user.{userId}@example.com",
         Username = "testuser",
         Firstname = "Test",
         Lastname = "User",
         Role = SDMISAppQG.Models.Enums.UserRole.Firefighter,
         IsDeleted = false
      };
      context.Users.Add(user);

      // 2. Create VehicleType
      var vehicleTypeId = Guid.NewGuid();
      var vehicleType = new VehicleTypeEntity {
         Id = vehicleTypeId,
         CreatedAt = DateTime.UtcNow,
         Label = "Test Type",
         CrewCapacity = 4
      };
      context.VehicleTypes.Add(vehicleType);

      // 3. Create Vehicle
      var vehicleId = Guid.NewGuid();
      var vehicle = new VehicleEntity {
         Id = vehicleId,
         IdHardware = new Random().Next(1000, 9999),
         Type = vehicleType,
         CreatedAt = DateTime.UtcNow,
         LastLocation = new Point(4.8357, 45.7640) { SRID = 4326 },
         Availability = VehicleAvailability.Available,
         UnavailabilityReason = null,
         Fuel = 100.0f,
         PassengerCount = 0
      };
      context.Vehicles.Add(vehicle);

      // 4. Assign User to Vehicle (Create Passenger)
      var passengerId = Guid.NewGuid();
      var passenger = new PassengerEntity {
         Id = passengerId,
         CreatedAt = DateTime.UtcNow,
         UserId = userId,
         VehicleId = vehicleId,
         User = user,
         Vehicle = vehicle
      };
      context.Passengers.Add(passenger);

      await context.SaveChangesAsync();

      // Act
      var result = await controller.GetCurrentVehicle(userId);

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnedVehicle = Assert.IsType<VehicleEntity>(okResult.Value);
      Assert.Equal(vehicleId, returnedVehicle.Id);
      Assert.Equal(vehicleTypeId, returnedVehicle.Type!.Id);

      // Cleanup
      context.Passengers.Remove(passenger);
      context.Vehicles.Remove(vehicle);
      context.VehicleTypes.Remove(vehicleType);
      context.Users.Remove(user);
      await context.SaveChangesAsync();
   }

   [Fact]
   public async Task CannotAssignOperatorToVehicle() {
      // Arrange
      var context = _fixture.Context;
      var passengerController = new SDMISAppQG.Controllers.PassengersController(context);

      // 1. Create Operator User
      var userId = Guid.NewGuid();
      var user = new UserEntity {
         Id = userId,
         CreatedAt = DateTime.UtcNow,
         KeyCloakId = Guid.NewGuid(),
         Email = $"operator.{userId}@example.com",
         Username = "operatoruser",
         Firstname = "Operator",
         Lastname = "User",
         Role = SDMISAppQG.Models.Enums.UserRole.Operator,
         IsDeleted = false
      };
      context.Users.Add(user);

      // 2. Create Vehicle
      var vehicleId = Guid.NewGuid();
      var typeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
      var vehicleType = await context.VehicleTypes.FindAsync(typeId);
      Assert.NotNull(vehicleType);

      var vehicle = new VehicleEntity {
         Id = vehicleId,
         IdHardware = new Random().Next(10000, 19999),
         Type = vehicleType,
         CreatedAt = DateTime.UtcNow,
         LastLocation = new Point(4.8357, 45.7640) { SRID = 4326 },
         Availability = VehicleAvailability.Available,
         UnavailabilityReason = null,
         Fuel = 100.0f,
         PassengerCount = 0
      };
      context.Vehicles.Add(vehicle);
      await context.SaveChangesAsync();

      var dto = new SDMISAppQG.Models.DTOs.CreatePassengerDto {
         UserId = userId,
         VehicleId = vehicleId
      };

      // Act
      var result = await passengerController.AssignPassenger(dto);

      // Assert
      var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
      Assert.Equal("Only users with the Firefighter role can be assigned to a vehicle.", badRequestResult.Value);

      // Cleanup
      context.Vehicles.Remove(vehicle);
      context.Users.Remove(user);
      await context.SaveChangesAsync();
   }
}
