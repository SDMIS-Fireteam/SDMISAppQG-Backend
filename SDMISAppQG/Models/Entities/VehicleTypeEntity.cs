using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

public class VehicleTypeEntity : BaseEntity {
   public required string Label { get; set; }
   public required int CrewCapacity { get; set; }
   public List<string>? Consumables { get; set; }

   /// <summary>
   /// Liste des IDs de types d'incidents compatibles (pour le moteur de décision)
   /// </summary>
   [NotMapped]
   public List<Guid> CompatibleIncidentIds { get; set; } = new();
}
