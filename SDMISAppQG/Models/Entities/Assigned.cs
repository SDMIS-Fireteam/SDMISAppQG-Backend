using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

/// <summary>
/// Table d'assignation entre une intervention et un véhicule
/// </summary>
public class Assigned : BaseEntity {
   public required Guid InterventionId { get; set; }
   public required Guid VehicleId { get; set; }

   /// <summary>
   /// Itinéraire du véhicule stocké en JSON
   /// </summary>
   [Column(TypeName = "jsonb")]
   public string? Itinerary { get; set; }

   public DateTime? Begin { get; set; }
   public DateTime? End { get; set; }
}
