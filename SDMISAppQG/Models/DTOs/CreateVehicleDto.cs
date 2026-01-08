using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un véhicule
/// </summary>
public class CreateVehicleDto
{
    /// <summary>
    /// Identifiant matériel unique du véhicule (du micro:bit ou dispositif embarqué)
    /// </summary>
    public required int IdHardware { get; set; }
    public required Guid TypeId { get; set; }
    public Point? LastLocation { get; set; }
    public required VehicleAvailability Availability { get; set; }
    public required VehicleUnavailabilityReason UnavailabilityReason { get; set; }
    public required float Fuel { get; set; }
}
