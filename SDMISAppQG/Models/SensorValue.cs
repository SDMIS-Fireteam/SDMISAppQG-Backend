using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models; 
/// <summary>
/// Représente une valeur d'un capteur à un instant donné
/// </summary>
public class SensorValue {
   public required string Code { get; set; } // ex: "TEMP_MOTEUR", "GPS_LAT"
   public double Valeur { get; set; } // ex: 85.5
   public required UnitOfMeasurement Unite { get; set; } // ex: "C", "deg"
   public Dictionary<string, string>? Metadatas { get; set; } // Pour le "au cas où"

   public static SensorValue FromTelemetry(string sensorName, object value) {
      // Ici, on pourrait avoir une logique plus complexe pour déterminer l'unité et le code
      string stringValue = value.ToString() ?? string.Empty;
      UnitOfMeasurement unit;
      switch(stringValue) {
         case string s when s.Contains("%"):
            unit = UnitOfMeasurement.Percentage;
            break;
         case string s when s.Contains("C"):
            unit = UnitOfMeasurement.Celsius;
            break;

         case string s when s.Contains("L"):
            unit = UnitOfMeasurement.VolumeLiters;
            break;
         default:
            unit = UnitOfMeasurement.Unknown;
            break;
      }
      return new SensorValue {
         Code = sensorName.ToUpperInvariant(),
         Valeur = Convert.ToDouble(value),
         Unite = unit,
      };
   }
}
