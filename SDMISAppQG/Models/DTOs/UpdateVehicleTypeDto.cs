namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un type de véhicule
/// </summary>
public class UpdateVehicleTypeDto
{
    public string? Label { get; set; }
    public int? CrewCapacity { get; set; }
}
