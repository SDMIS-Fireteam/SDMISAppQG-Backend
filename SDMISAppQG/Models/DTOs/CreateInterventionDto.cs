using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'une intervention
/// </summary>
public class CreateInterventionDto
{
    public required Guid IncidentId { get; set; }
    public Guid? VehicleId { get; set; }
    public required DateTime Begin { get; set; }
    public DateTime? End { get; set; }
    public required InterventionStatus Status { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public Guid? ConfirmedBy { get; set; }
}
