using SDMISAppQG.Models.Messaging;

namespace SDMISTests.Unit;

public class MessageEnvelopeTests {
   [Fact]
   public void Constructor_InitializesDefaultValues() {
      // Act
      var envelope = new MessageEnvelope<string>();

      // Assert
      Assert.NotNull(envelope.MessageId);
      Assert.NotEmpty(envelope.MessageId);
      Assert.Equal("String", envelope.MessageType);
      Assert.Equal("dotnet-backend", envelope.Source);
      Assert.True(envelope.Timestamp <= DateTime.UtcNow);
   }

   [Fact]
   public void Data_SetAndGet_WorksCorrectly() {
      // Arrange
      var data = "Test Data";
      var envelope = new MessageEnvelope<string> { Data = data };

      // Assert
      Assert.Equal(data, envelope.Data);
   }
}
