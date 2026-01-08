using System.ComponentModel.DataAnnotations;

namespace SDMISAppQG.Models.DTOs;

public class CreateAssignedDto
{
    [Required]
    public Guid InterventionId { get; set; }
    
    [Required]
    public Guid VehicleId { get; set; }
    
    public string? Itinerary { get; set; }
    
    public DateTime? Begin { get; set; }
    
    public DateTime? End { get; set; }
}
