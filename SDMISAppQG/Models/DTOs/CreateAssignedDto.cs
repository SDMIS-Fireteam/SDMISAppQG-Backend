using System.ComponentModel.DataAnnotations;

namespace SDMISAppQG.Models.DTOs;

public class CreateAssignedDto
{
    public required Guid InterventionId { get; set; }

    public required Guid VehicleId { get; set; }

    public string? Itinerary { get; set; }

    public DateTime? Begin { get; set; }

    public DateTime? End { get; set; }
}
