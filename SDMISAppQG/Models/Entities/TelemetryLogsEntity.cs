using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace SDMISAppQG.Models.Entities; 
public class TelemetryLogsEntity : BaseEntity {
   public required Guid VehicleId { get; set; }
   [Column(TypeName = "geography(Point, 4326)")]
   public required Point Position { get; set; }
   public required DateTime Timestamp { get; set; }
   [Column(TypeName = "jsonb")]
   public required List<SensorValue> SensorsValues { get; set; }
}
