using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'une intervention
/// </summary>
public class UpdateInterventionDto
{
    public Guid? IncidentId { get; set; }
    public DateTime? Begin { get; set; }
    public DateTime? End { get; set; }
    public InterventionStatus? Status { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public Guid? ConfirmedBy { get; set; }
}
