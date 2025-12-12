namespace SDMISAppQG.Models.Entities; 
/// <summary>
/// Table pour savoir si le véhicule est adapté à un ou plusieurs type d'incidents
/// </summary>
public class AdaptedVehicleIncidentEntity : BaseEntity {
   public required Guid VehicleId { get; set; }
   public required Guid IncidentId { get; set; }

}
