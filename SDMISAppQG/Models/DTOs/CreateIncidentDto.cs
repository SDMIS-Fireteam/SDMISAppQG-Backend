using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Incidents;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un incident
/// </summary>
public class CreateIncidentDto
{
    public required Guid TypeId { get; set; }
    public required Point Location { get; set; }
    public required IncidentSeverity Severity { get; set; }
    public required float Priority { get; set; }
    public required IncidentStatus Status { get; set; }
    public required IncidentSource Source { get; set; }
    public string? Description { get; set; }
}
