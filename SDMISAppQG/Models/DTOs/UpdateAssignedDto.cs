namespace SDMISAppQG.Models.DTOs;

public class UpdateAssignedDto
{
    public Guid? InterventionId { get; set; }

    public Guid? VehicleId { get; set; }

    public string? Itinerary { get; set; }

    public DateTime? Begin { get; set; }

    public DateTime? End { get; set; }
}
