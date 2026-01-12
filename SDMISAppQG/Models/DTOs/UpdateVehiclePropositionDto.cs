namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'une proposition de véhicule
/// </summary>
public class UpdateVehiclePropositionDto
{
    public Guid? IncidentId { get; set; }
    public Guid? VehicleId { get; set; }
    public float? Score { get; set; }
}
