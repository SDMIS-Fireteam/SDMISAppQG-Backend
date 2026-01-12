using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SDMISAppQG.Controllers;
using SDMISAppQG.Infrastructure.Services.RabbitMQ;

namespace SDMISTests.Unit;

public class MessagingControllerTests {
   [Fact]
   public void HealthCheck_ReturnsOk() {
      // Arrange
      var mockService = new Mock<IRabbitMQService>();
      var mockLogger = new Mock<ILogger<MessagingController>>();
      var controller = new MessagingController(mockService.Object, mockLogger.Object);

      // Act
      var result = controller.HealthCheck();

      // Assert
      var okResult = Assert.IsType<OkObjectResult>(result);
      var value = okResult.Value;
      Assert.NotNull(value);
   }

   [Fact]
   public void SendVehicleLocation_CallsPublish_AndReturnsOk() {
      // Arrange
      var mockService = new Mock<IRabbitMQService>();
      var mockLogger = new Mock<ILogger<MessagingController>>();
      var controller = new MessagingController(mockService.Object, mockLogger.Object);

      var update = new SDMISAppQG.Models.Messaging.VehicleLocationUpdate {
         VehicleId = Guid.NewGuid(),
         Latitude = 45.0,
         Longitude = 4.0,
         Timestamp = DateTime.UtcNow
      };

      // Act
      var result = controller.SendVehicleLocation(update);

      // Assert
      // Verify PublishToJava was called once
      mockService.Verify(s => s.PublishToJava(It.IsAny<SDMISAppQG.Models.Messaging.MessageEnvelope<SDMISAppQG.Models.Messaging.VehicleLocationUpdate>>()), Times.Once);

      var okResult = Assert.IsType<OkObjectResult>(result);
      var value = okResult.Value;
      Assert.NotNull(value);
   }
}
