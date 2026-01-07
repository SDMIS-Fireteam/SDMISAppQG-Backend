using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

[Index(nameof(IdHardware), IsUnique = true)]
public class VehicleEntity : BaseEntity {
   /// <summary>
   /// Identifiant matériel unique du véhicule (du micro:bit ou dispositif embarqué)
   /// </summary>
   public int IdHardware { get; set; }
   
   public VehicleType Type { get; set; }
   [Column(TypeName = "geography(Point, 4326)")]
   public Point? LastLocation { get; set; }
   public VehicleAvailability Availability { get; set; }
   public VehicleUnavailabilityReason UnavailabilityReason { get; set; }
   public float Fuel { get; set; }
}
