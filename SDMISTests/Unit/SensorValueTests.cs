using SDMISAppQG.Models;
using SDMISAppQG.Models.Enums;
using Xunit;

namespace SDMISTests.Unit;

public class SensorValueTests {
   [Fact]
   public void FromTelemetry_UnknownUnit_ReturnsUnknown() {
      var result = SensorValue.FromTelemetry("speed", 50.5);

      Assert.Equal("SPEED", result.Code);
      Assert.Equal(50.5, result.Valeur);
      Assert.Equal(UnitOfMeasurement.Unknown, result.Unite);
   }

   [Fact]
   public void FromTelemetry_WithPercentageString_ReturnsCorrectValueAndUnit() {
      var result = SensorValue.FromTelemetry("fuel", "85.5%");

      Assert.Equal("FUEL", result.Code);
      Assert.Equal(85.5, result.Valeur);
      Assert.Equal(UnitOfMeasurement.Percentage, result.Unite);
   }
}
