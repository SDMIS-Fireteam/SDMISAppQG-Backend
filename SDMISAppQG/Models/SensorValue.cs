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
}
