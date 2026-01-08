namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un type de véhicule
/// </summary>
public class CreateVehicleTypeDto
{
    public required string Label { get; set; }
    public required int CrewCapacity { get; set; }
}
