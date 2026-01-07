using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Incidents;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

public class IncidentEntity : BaseEntity
{
    public required IncidentTypeEntity Type { get; set; }
    [Column(TypeName = "geography(Point, 4326)")]
    public required Point Location { get; set; }
    public required IncidentSeverity Severity { get; set; }
    public required float Priority { get; set; }
    public required IncidentStatus Status { get; set; }
    public required IncidentSource Source { get; set; }
    public string? Description { get; set; }
}
