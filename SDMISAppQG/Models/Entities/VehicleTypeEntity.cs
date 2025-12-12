namespace SDMISAppQG.Models.Entities; 
/// <summary>
/// Table qui référence les différents type de véhicules
/// </summary>
public class VehicleTypeEntity : BaseEntity {
   public required string Label { get; set; }
   public required int CrewCapacity { get; set; }
}
