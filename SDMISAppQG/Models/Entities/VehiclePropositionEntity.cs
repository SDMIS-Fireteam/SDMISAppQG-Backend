using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

/// <summary>
/// Proposition de véhicule pour une intervention
/// </summary>
public class VehiclePropositionEntity : BaseEntity
{
    public required Guid InterventionId { get; set; }
    public required Guid VehicleId { get; set; }
    public required float Score { get; set; }
}