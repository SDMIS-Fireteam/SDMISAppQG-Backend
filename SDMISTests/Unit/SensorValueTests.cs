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

       public void FromTelemetry_WithPercentageString_ReturnsCorrectValueAndUnit()

       {

           var result = SensorValue.FromTelemetry("fuel", "85.5%");

           

           Assert.Equal("FUEL", result.Code);

           Assert.Equal(85.5, result.Valeur);

           Assert.Equal(UnitOfMeasurement.Percentage, result.Unite);

       }

   

       [Fact]

       public void FromTelemetry_WithCelsiusString_ReturnsCorrectValueAndUnit()

       {

           var result = SensorValue.FromTelemetry("temp", "25.0 C");

           

           Assert.Equal("TEMP", result.Code);

           Assert.Equal(25.0, result.Valeur);

           Assert.Equal(UnitOfMeasurement.Celsius, result.Unite);

       }

   

       [Fact]

       public void FromTelemetry_WithLitersString_ReturnsCorrectValueAndUnit()

       {

           var result = SensorValue.FromTelemetry("water", "1000L");

           

           Assert.Equal("WATER", result.Code);

           Assert.Equal(1000.0, result.Valeur);

           Assert.Equal(UnitOfMeasurement.VolumeLiters, result.Unite);

       }

   

           [Theory]

   

           [InlineData(100, 100.0)]

   

           [InlineData(100.5f, 100.5)]

   

           [InlineData("100.5", 100.5)]

   

           public void FromTelemetry_WithVariousNumericTypes_ParsesCorrectly(object value, double expected)

   

           {

   

               var result = SensorValue.FromTelemetry("test", value);

   

               Assert.Equal(expected, result.Valeur);

   

           }

   

       

   

           [Fact]

   

           public void FromTelemetry_WithEmptyString_ReturnsZeroAndUnknown()

   

           {

   

               var result = SensorValue.FromTelemetry("empty", "");

   

               Assert.Equal("EMPTY", result.Code);

   

               Assert.Equal(0, result.Valeur); // Convert.ToDouble("") throws FormatException usually? Let's verify behavior.

   

               Assert.Equal(UnitOfMeasurement.Unknown, result.Unite);

   

           }

   

       

   

           [Fact]

   

           public void FromTelemetry_WithNull_ReturnsZeroAndUnknown()

   

           {

   

               var result = SensorValue.FromTelemetry("null", null!);

   

               Assert.Equal("NULL", result.Code);

   

               Assert.Equal(0, result.Valeur);

   

               Assert.Equal(UnitOfMeasurement.Unknown, result.Unite);

   

           }

   

       

   

           [Fact]

   

           public void FromTelemetry_WithUnknownUnitString_ReturnsValueAndUnknown()

   

           {

   

               // Example: "100.5 XYZ" -> "100.5 XYZ" -> Convert fails?

   

               // Current logic only strips %, C, L.

   

               // If I pass "100.5 XYZ", it goes to default case, unit=Unknown.

   

               // cleanValue = "100.5 XYZ". Convert.ToDouble("100.5 XYZ") -> FormatException.

   

               // So this confirms the limitation of the current parsing logic.

   

               // I will Assert.Throws<FormatException> for this case.

   

               Assert.Throws<FormatException>(() => SensorValue.FromTelemetry("unknown", "100.5 XYZ"));

   

           }

   

       }
