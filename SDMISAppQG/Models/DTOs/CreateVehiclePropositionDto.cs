namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'une proposition de véhicule
/// </summary>
public class CreateVehiclePropositionDto
{
    public required Guid InterventionId { get; set; }
    public required Guid VehicleId { get; set; }
    public required float Score { get; set; }
}
