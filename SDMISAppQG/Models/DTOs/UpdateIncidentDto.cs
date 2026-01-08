using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Incidents;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un incident
/// </summary>
public class UpdateIncidentDto
{
    public Guid? TypeId { get; set; }
    public Point? Location { get; set; }
    public IncidentSeverity? Severity { get; set; }
    public float? Priority { get; set; }
    public IncidentStatus? Status { get; set; }
    public IncidentSource? Source { get; set; }
    public string? Description { get; set; }
}
