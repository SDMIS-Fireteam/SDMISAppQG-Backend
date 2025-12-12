using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities; 
public class VehicleEntity : BaseEntity {
   public VehicleType Type { get; set; }
   [Column(TypeName = "geography(Point, 4326)")]
   public Point? LastLocation { get; set; }
   public VehicleAvailability Availability { get; set; }
   public VehicleUnavailabilityReason UnavailabilityReason { get; set; }
   public float Fuel { get; set; }
}
