using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities; 
public class TelemetryLog : BaseEntity {
   public required Guid TruckId { get; set; }
   [Column(TypeName = "jsonb")]
   public required List<SensorValue> SensorsValues { get; set; }
}
