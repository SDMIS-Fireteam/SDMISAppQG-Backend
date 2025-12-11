using Newtonsoft.Json.Linq;

namespace SDMISAppQG.Models; 
public class TelemetryLog : BaseEntity {
   public required Guid TruckId { get; set; }
   public required JToken SensorsValues { get; set; }
}
