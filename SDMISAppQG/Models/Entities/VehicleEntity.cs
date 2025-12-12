using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities; 
public class VehicleEntity : BaseEntity {
   public required VehicleTypeEntity Type { get; set; }
   [Column(TypeName = "geography(Point, 4326)")]
   /// <summary>
   /// The matriculation of the vehicle.
   /// </summary>
   public required string Matriculation { get; set; }

   /// <summary>
   /// The VIN of the vehicle.
   /// </summary>
   public required string Vin { get; set; }
   public Point? LastLocation { get; set; }
   public VehicleAvailability Availability { get; set; }
   public VehicleUnavailabilityReason UnavailabilityReason { get; set; }
   public float Fuel { get; set; }
}
