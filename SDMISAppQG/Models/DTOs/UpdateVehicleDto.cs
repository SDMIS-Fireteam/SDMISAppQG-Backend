using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un véhicule
/// </summary>
public class UpdateVehicleDto
{
    public int? IdHardware { get; set; }
    public Guid? TypeId { get; set; }
    public Point? LastLocation { get; set; }
    public VehicleAvailability? Availability { get; set; }
    public VehicleUnavailabilityReason? UnavailabilityReason { get; set; }
    public float? Fuel { get; set; }
    public string? Consumable { get; set; }
    public int? PassengerCount { get; set; }
}
