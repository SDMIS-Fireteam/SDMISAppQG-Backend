namespace SDMISAppQG.Models.Entities;

/// <summary>
/// Proposition de véhicule pour une intervention
/// </summary>
public class VehiclePropositionEntity : BaseEntity {
   public required IncidentEntity Incident { get; set; }
   public required VehicleEntity Vehicle { get; set; }
   public required float Score { get; set; }
}